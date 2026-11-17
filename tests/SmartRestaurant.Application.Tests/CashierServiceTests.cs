namespace SmartRestaurant.Application.Tests;

using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SmartRestaurant.Application.DTOs;
using SmartRestaurant.Application.Services;
using SmartRestaurant.Domain.Entities;
using SmartRestaurant.Domain.Enums;
using SmartRestaurant.Domain.Exceptions;
using SmartRestaurant.Infrastructure.Data;
using Xunit;

public class CashierServiceTests
{
    private readonly Guid _branchId = Guid.NewGuid();
    private readonly Guid _cashierId = Guid.NewGuid();

    private (AppDbContext db, CashierService service, RestaurantTable table, Order order, WorkShift shift) CreateTestFixture(string dbName, bool isVipRoom = false)
    {
        var db = TestDbContextFactory.Create(dbName);
        var service = new CashierService(db);

        var branch = new Branch("Chi nhánh Test", "Hà Nội", "0901234567", TimeSpan.FromHours(6));
        db.Branches.Add(branch);

        var area = new DiningArea(_branchId, "Khu Vực Trong Nhà", AreaType.Indoor, 1);
        db.DiningAreas.Add(area);

        DiningRoom? room = null;
        if (isVipRoom)
        {
            room = new DiningRoom(area.Id, "Phòng VIP Hoàng Gia", true, 10.0m, 50000m); // 10% + 50k surcharge per PDR-011
            db.DiningRooms.Add(room);
        }

        var table = new RestaurantTable(_branchId, area.Id, "TB-01", "Bàn Tiêu Chuẩn 1", 4, room?.Id);
        table.Occupy();
        table.RequestBilling();
        db.RestaurantTables.Add(table);

        var cashier = new StaffMember(_branchId, "Nguyễn Thu Ngân", "cashier01", "cashier@restaurant.vn", StaffRole.Cashier, "1111");
        db.StaffMembers.Add(cashier);

        var shift = new WorkShift(_branchId, cashier.Id, openingCashFloat: 1000000m);
        db.WorkShifts.Add(shift);

        var cat = new MenuCategory(_branchId, "Món Ăn Chính", "", 1);
        db.MenuCategories.Add(cat);

        var steakItem = new MenuItem(_branchId, cat.Id, "STEAK-01", "Bò Bít Tết Thượng Hạng", 200000m);
        var saladItem = new MenuItem(_branchId, cat.Id, "SALAD-01", "Salad Hoàng Đế", 100000m);
        db.MenuItems.AddRange(steakItem, saladItem);

        var session = new DiningSession(_branchId, table.Id, "SS-CASHIER-01", 4);
        db.DiningSessions.Add(session);

        var order = new Order(_branchId, session.Id, "OD-CASHIER-01", DateTime.UtcNow);
        db.Orders.Add(order);

        var round = new OrderRound(order.Id, 1, cashier.Id, Guid.NewGuid().ToString());
        db.OrderRounds.Add(round);

        var item1 = new OrderItem(round.Id, steakItem.Id, steakItem.Name, steakItem.BasePrice, 2); // 400k
        var item2 = new OrderItem(round.Id, saladItem.Id, saladItem.Name, saladItem.BasePrice, 1); // 100k
        db.OrderItems.AddRange(item1, item2);

        db.SaveChanges();

        return (db, service, table, order, shift);
    }

    [Fact]
    public async Task GetBillCalculationAsync_ShouldComputeSubtotal_Vat_AndSurchargesCorrectly()
    {
        // VIP room with 10% surcharge + 50k fixed
        var (db, service, table, order, _) = CreateTestFixture(Guid.NewGuid().ToString(), isVipRoom: true);

        var bill = await service.GetBillCalculationAsync(order.Id);

        bill.Should().NotBeNull();
        bill.Subtotal.Should().Be(500000m); // 200k*2 + 100k*1
        bill.SurchargeRate.Should().Be(10.0m);
        // Surcharge = 500k * 10% + 50k = 100k
        bill.SurchargeAmount.Should().Be(100000m);
        bill.TaxableAmount.Should().Be(600000m); // 500k + 100k
        bill.VatRate.Should().Be(8.0m);
        bill.VatAmount.Should().Be(48000m); // 600k * 8%
        bill.TotalAmount.Should().Be(648000m); // 600k + 48k
        bill.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task CreateStandardInvoiceAsync_ShouldCreatePendingInvoice()
    {
        var (db, service, table, order, _) = CreateTestFixture(Guid.NewGuid().ToString(), isVipRoom: false);

        var invoice = await service.CreateStandardInvoiceAsync(order.Id, _cashierId);

        invoice.Should().NotBeNull();
        invoice.Subtotal.Should().Be(500000m);
        invoice.SurchargeAmount.Should().Be(0m);
        invoice.VatAmount.Should().Be(40000m); // 500k * 8%
        invoice.TotalAmount.Should().Be(540000m);
        invoice.Status.Should().Be(InvoiceStatus.Pending);
        invoice.IsSettled.Should().BeFalse();

        var dbInvoice = await db.Invoices.FirstOrDefaultAsync(i => i.Id == invoice.InvoiceId);
        dbInvoice.Should().NotBeNull();
    }

    [Fact]
    public async Task SettleInvoiceAsync_WithCash_ShouldAddSystemCash_AndReleaseTable()
    {
        var (db, service, table, order, shift) = CreateTestFixture(Guid.NewGuid().ToString(), isVipRoom: false);

        var invoice = await service.CreateStandardInvoiceAsync(order.Id, _cashierId);

        var settleResult = await service.SettleInvoiceAsync(new SettleInvoiceRequest(
            InvoiceId: invoice.InvoiceId,
            PaymentMethod: PaymentMethod.Cash,
            Amount: 600000m,
            ChangeAmount: 60000m, // Change given
            TransactionRef: "CASH-PAID",
            CashierStaffId: _cashierId
        ));

        settleResult.IsSettled.Should().BeTrue();
        settleResult.Status.Should().Be(InvoiceStatus.Paid);
        settleResult.PaidAmount.Should().Be(600000m);
        settleResult.ChangeAmount.Should().Be(60000m);

        // Verify shift system cash updated (net 540k)
        var updatedShift = await db.WorkShifts.FirstAsync(s => s.Id == shift.Id);
        updatedShift.SystemCashTotal.Should().Be(540000m);

        // Verify table released to Available
        var updatedTable = await db.RestaurantTables.FirstAsync(t => t.Id == table.Id);
        updatedTable.Status.Should().Be(TableStatus.Available);

        var outbox = await db.OutboxMessages.FirstOrDefaultAsync(m => m.EventType == "TableStatusChanged");
        outbox.Should().NotBeNull();
    }

    [Fact]
    public async Task SettleInvoiceAsync_AlreadySettled_ShouldThrowException_OD13()
    {
        var (db, service, table, order, shift) = CreateTestFixture(Guid.NewGuid().ToString(), isVipRoom: false);

        var invoice = await service.CreateStandardInvoiceAsync(order.Id, _cashierId);

        await service.SettleInvoiceAsync(new SettleInvoiceRequest(
            InvoiceId: invoice.InvoiceId,
            PaymentMethod: PaymentMethod.Cash,
            Amount: 540000m,
            ChangeAmount: 0m,
            TransactionRef: null,
            CashierStaffId: _cashierId
        ));

        // Attempting to settle again must throw per OD-13
        var act = async () => await service.SettleInvoiceAsync(new SettleInvoiceRequest(
            InvoiceId: invoice.InvoiceId,
            PaymentMethod: PaymentMethod.Cash,
            Amount: 540000m,
            ChangeAmount: 0m,
            TransactionRef: null,
            CashierStaffId: _cashierId
        ));

        await act.Should().ThrowAsync<DomainValidationException>()
            .WithMessage("*OD-13*");
    }

    [Fact]
    public async Task SplitBillAsync_ModeEqual_ShouldNotReleaseTableUntilAllSubBillsSettled_PDR009()
    {
        var (db, service, table, order, shift) = CreateTestFixture(Guid.NewGuid().ToString(), isVipRoom: false);

        // Split into 2 equal parts
        var splitInvoices = await service.SplitBillAsync(new SplitBillRequest(
            OrderId: order.Id,
            SplitMode: "equal",
            EqualParts: 2,
            Allocations: null,
            CashierStaffId: _cashierId
        ));

        splitInvoices.Should().HaveCount(2);

        // 1. Settle Sub-Bill 1
        var settle1 = await service.SettleInvoiceAsync(new SettleInvoiceRequest(
            InvoiceId: splitInvoices[0].InvoiceId,
            PaymentMethod: PaymentMethod.PosCard,
            Amount: splitInvoices[0].TotalAmount,
            ChangeAmount: 0,
            TransactionRef: "CARD-SLIP-01",
            CashierStaffId: _cashierId
        ));

        settle1.IsSettled.Should().BeTrue();

        // Table must STILL be in BillingRequested (NOT released) because Sub-Bill 2 is unpaid!
        var tableAfterFirstBill = await db.RestaurantTables.FirstAsync(t => t.Id == table.Id);
        tableAfterFirstBill.Status.Should().Be(TableStatus.BillingRequested);

        // 2. Settle Sub-Bill 2
        var settle2 = await service.SettleInvoiceAsync(new SettleInvoiceRequest(
            InvoiceId: splitInvoices[1].InvoiceId,
            PaymentMethod: PaymentMethod.VietQR,
            Amount: splitInvoices[1].TotalAmount,
            ChangeAmount: 0,
            TransactionRef: "QR-TRANS-02",
            CashierStaffId: _cashierId
        ));

        settle2.IsSettled.Should().BeTrue();

        // Now that 100% of sub-bills are settled, table is released!
        var tableAfterAllBills = await db.RestaurantTables.FirstAsync(t => t.Id == table.Id);
        tableAfterAllBills.Status.Should().Be(TableStatus.Available);
    }

    [Fact]
    public async Task ProcessVietQrWebhookAsync_ShouldAutoSettlePendingInvoice()
    {
        var (db, service, table, order, shift) = CreateTestFixture(Guid.NewGuid().ToString(), isVipRoom: false);

        var invoice = await service.CreateStandardInvoiceAsync(order.Id, _cashierId);

        var success = await service.ProcessVietQrWebhookAsync(new ReconcileWebhookRequest(
            TransactionRef: "NAPAS-FT261116001",
            Amount: invoice.TotalAmount,
            OrderCode: invoice.InvoiceNumber,
            BankCode: "MB",
            PayerInfo: "NGUYEN VAN KHACH"
        ));

        success.Should().BeTrue();

        var updatedInvoice = await db.Invoices.FirstAsync(i => i.Id == invoice.InvoiceId);
        updatedInvoice.IsSettled.Should().BeTrue();
        updatedInvoice.PaymentMethod.Should().Be(PaymentMethod.VietQR);
    }

    [Fact]
    public async Task ProcessVietQrWebhookAsync_WhenAlreadySettled_ShouldRecordSuspenseLedger_PDR008()
    {
        var (db, service, table, order, shift) = CreateTestFixture(Guid.NewGuid().ToString(), isVipRoom: false);

        var invoice = await service.CreateStandardInvoiceAsync(order.Id, _cashierId);

        // Settle by Cash first
        await service.SettleInvoiceAsync(new SettleInvoiceRequest(
            InvoiceId: invoice.InvoiceId,
            PaymentMethod: PaymentMethod.Cash,
            Amount: invoice.TotalAmount,
            ChangeAmount: 0,
            TransactionRef: "CASH-DONE",
            CashierStaffId: _cashierId
        ));

        // Now delayed VietQR webhook arrives
        var webhookProcessed = await service.ProcessVietQrWebhookAsync(new ReconcileWebhookRequest(
            TransactionRef: "DELAYED-FT9999",
            Amount: invoice.TotalAmount,
            OrderCode: invoice.InvoiceNumber,
            BankCode: "VCB",
            PayerInfo: "DELAYED PAYER"
        ));

        webhookProcessed.Should().BeTrue();

        // Must record in SuspenseLedger per PDR-008
        var suspense = await db.SuspenseLedgers.FirstOrDefaultAsync(s => s.TransactionRef == "DELAYED-FT9999");
        suspense.Should().NotBeNull();
        suspense!.Status.Should().Be(SuspenseLedgerStatus.Unreconciled);
        suspense.Reason.Should().Contain("trước đó");
    }

    [Fact]
    public async Task CloseShiftBlindAsync_ShouldRecordVariance_AndCreateVarianceLog()
    {
        var (db, service, table, order, shift) = CreateTestFixture(Guid.NewGuid().ToString(), isVipRoom: false);

        // Settle an invoice for 540,000đ cash
        var invoice = await service.CreateStandardInvoiceAsync(order.Id, _cashierId);
        await service.SettleInvoiceAsync(new SettleInvoiceRequest(
            InvoiceId: invoice.InvoiceId,
            PaymentMethod: PaymentMethod.Cash,
            Amount: 540000m,
            ChangeAmount: 0m,
            TransactionRef: null,
            CashierStaffId: _cashierId
        ));

        // Opening: 1,000,000 + Revenue: 540,000 = Expected: 1,540,000đ
        // Cashier blind count: 1,520,000đ (Short by 20,000đ)
        var result = await service.CloseShiftBlindAsync(new BlindShiftCloseRequest(
            ShiftId: shift.Id,
            ActualCashCount: 1520000m,
            Notes: "Thiếu 20k do tiền lẻ thối sót",
            CashierStaffId: _cashierId
        ));

        result.ExpectedAmount.Should().Be(1540000m);
        result.ActualCashCount.Should().Be(1520000m);
        result.VarianceAmount.Should().Be(-20000m);
        result.HasDiscrepancy.Should().BeTrue();

        var log = await db.ShiftClosingVarianceLogs.FirstOrDefaultAsync(l => l.ShiftId == shift.Id);
        log.Should().NotBeNull();
        log!.VarianceAmount.Should().Be(-20000m);
        log.CashierNotes.Should().Be("Thiếu 20k do tiền lẻ thối sót");
    }
}
