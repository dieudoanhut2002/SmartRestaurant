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

public class EndToEndWorkflowTests
{
    private readonly Guid _branchId = Guid.NewGuid();
    private readonly Guid _managerId = Guid.NewGuid();
    private readonly Guid _waiterId = Guid.NewGuid();
    private readonly Guid _cashierId = Guid.NewGuid();
    private readonly Guid _chefId = Guid.NewGuid();

    private (
        AppDbContext db,
        PosOrderService posService,
        KitchenService kitchenService,
        CashierService cashierService,
        VietQrService vietQrService,
        RestaurantTable table,
        DiningRoom vipRoom,
        MenuItem steakDish,
        MenuItem beerDrink,
        MenuItem waterDrink,
        InventoryItem beerInventory,
        KitchenStation hotStation
    ) CreateIntegrationFixture(string dbName)
    {
        var db = TestDbContextFactory.Create(dbName);
        var posService = new PosOrderService(db);
        var kitchenService = new KitchenService(db);
        var cashierService = new CashierService(db);
        var vietQrService = new VietQrService();

        // 1. Branch
        var branch = new Branch(
            "SmartRestaurant Flagship Store",
            "123 Nguyễn Huệ, Quận 1, TP. Hồ Chí Minh",
            "028 3822 9999",
            new TimeSpan(6, 0, 0)
        );
        db.Branches.Add(branch);

        // 2. Staff Accounts
        var manager = new StaffMember(_branchId, "Nguyễn Quản Lý", "manager01", "mgr@restaurant.vn", StaffRole.RestaurantManager, "1234");
        var waiter = new StaffMember(_branchId, "Trần Bồi Bàn", "waiter01", "waiter@restaurant.vn", StaffRole.Waiter, "2222");
        var cashier = new StaffMember(_branchId, "Lê Thu Ngân", "cashier01", "cashier@restaurant.vn", StaffRole.Cashier, "1111");
        var chef = new StaffMember(_branchId, "Phạm Bếp Trưởng", "chef01", "chef@restaurant.vn", StaffRole.Chef, "3333");
        db.StaffMembers.AddRange(manager, waiter, cashier, chef);

        // 3. Dining Area & VIP Room (10% rate + 50k fixed surcharge per PDR-011)
        var area = new DiningArea(_branchId, "Tầng VIP Hoàng Gia", AreaType.VIP, 1);
        db.DiningAreas.Add(area);

        var vipRoom = new DiningRoom(area.Id, "Phòng Khánh Tiết VIP 1", true, 10.0m, 50000m);
        db.DiningRooms.Add(vipRoom);

        var table = new RestaurantTable(_branchId, area.Id, "VIP-01", "Bàn Tiệc VIP 01", 8, vipRoom.Id);
        db.RestaurantTables.Add(table);

        // 4. Kitchen Station
        var hotStation = new KitchenStation(_branchId, "HOT-KITCHEN", "Bếp Nóng Chuyên Trách", PrepStation.HotKitchen);
        db.KitchenStations.Add(hotStation);

        // 5. Menu Categories & Items
        var catFood = new MenuCategory(_branchId, "Món Chính Đặc Sắc", "", 1);
        var catDrink = new MenuCategory(_branchId, "Thức Uống & Bia", "", 2);
        db.MenuCategories.AddRange(catFood, catDrink);

        // Steak: cooked in HotKitchen, not direct FOH
        var steakDish = new MenuItem(_branchId, catFood.Id, "STEAK-WAGYU", "Bò Wagyu A5 Nướng Đá", 500000m, directFOHPick: false);
        steakDish.AssignToStation(hotStation.Id, PrepStation.HotKitchen);

        // Countable Beer: tracked in inventory
        var beerDrink = new MenuItem(_branchId, catDrink.Id, "BEER-CRAFT", "Bia Thủ Công Pasteur Street", 80000m, directFOHPick: false, isCountable: true);
        var beerInventory = new InventoryItem(_branchId, "INV-BEER", "Bia Pasteur Street", "Bia", "Chai", initialPhysicalStock: 50, minimumThreshold: 5, menuItemId: beerDrink.Id);
        db.InventoryItems.Add(beerInventory);

        // Direct FOH Pick Water: served directly by waiter without sending ticket to kitchen (PDR-012)
        var waterDrink = new MenuItem(_branchId, catDrink.Id, "WATER-MINERAL", "Nước Khoáng Alba Tự Nhiên", 35000m, directFOHPick: true);

        db.MenuItems.AddRange(steakDish, beerDrink, waterDrink);
        db.SaveChanges();

        return (db, posService, kitchenService, cashierService, vietQrService, table, vipRoom, steakDish, beerDrink, waterDrink, beerInventory, hotStation);
    }

    [Fact]
    public async Task CompleteDiningLifecycle_EndToEnd_AllInvariantsPreserved()
    {
        var fixture = CreateIntegrationFixture(Guid.NewGuid().ToString());
        var db = fixture.db;
        var pos = fixture.posService;
        var kds = fixture.kitchenService;
        var cashier = fixture.cashierService;
        var qrService = fixture.vietQrService;

        // =========================================================================
        // STEP 1: Cashier opens work shift with 1,000,000đ cash float
        // =========================================================================
        var shift = new WorkShift(fixture.table.BranchId, _cashierId, openingCashFloat: 1000000m, "Ca sáng đầu tuần");
        db.WorkShifts.Add(shift);
        await db.SaveChangesAsync();

        shift.Status.Should().Be(ShiftStatus.Open);
        shift.OpeningCashFloat.Should().Be(1000000m);
        shift.SystemCashTotal.Should().Be(0m);

        // =========================================================================
        // STEP 2: Waiter opens dining session on VIP Table
        // =========================================================================
        var openResult = await pos.OpenDiningSessionAsync(new OpenDiningSessionRequest(
            TableId: fixture.table.Id,
            GuestCount: 4,
            StaffId: _waiterId
        ));

        openResult.Should().NotBeNull();
        openResult.SessionId.Should().NotBeEmpty();

        var tableAfterOpen = await db.RestaurantTables.FirstAsync(t => t.Id == fixture.table.Id);
        tableAfterOpen.Status.Should().Be(TableStatus.Occupied);

        // =========================================================================
        // STEP 3: Waiter places order and fires order to kitchen (FireOrder)
        // Order: 2x Wagyu Steak, 4x Craft Beer (Countable), 2x Alba Water (Direct FOH)
        // =========================================================================
        string idempotencyKey = Guid.NewGuid().ToString();
        var fireRequest = new FireOrderRequest(
            TableId: fixture.table.Id,
            OrderId: null,
            IdempotencyKey: idempotencyKey,
            StaffId: _waiterId,
            Note: "Bò làm tái vừa (Medium Rare)",
            Items: new List<FireOrderItemDraftDto>
            {
                new(fixture.steakDish.Id, null, fixture.steakDish.Name, fixture.steakDish.BasePrice, 2, "Tái vừa, nhiều tiêu", null),
                new(fixture.beerDrink.Id, null, fixture.beerDrink.Name, fixture.beerDrink.BasePrice, 4, "Ướp lạnh, kèm ly đá", null),
                new(fixture.waterDrink.Id, null, fixture.waterDrink.Name, fixture.waterDrink.BasePrice, 2, "Không đá", null)
            }
        );

        var roundDto = await pos.FireOrderAsync(fireRequest);
        roundDto.Should().NotBeNull();
        roundDto.RoundNumber.Should().Be(1);

        // --- Verify Invariant 1: Countable Beer reserved stock in inventory (D2 / D5) ---
        var beerInv = await db.InventoryItems.FirstAsync(i => i.Id == fixture.beerInventory.Id);
        beerInv.PhysicalStock.Should().Be(50);
        beerInv.ReservedStock.Should().Be(4); // 4 bottles reserved!
        beerInv.AvailableStock.Should().Be(46); // Available = 50 - 4 = 46

        // --- Verify Invariant 2: Water with DirectFOHPick bypassed KDS (PDR-012) ---
        var tickets = await db.KitchenTickets.Where(t => t.OrderRoundId == roundDto.RoundId).ToListAsync();
        tickets.Should().HaveCount(1); // Only 1 ticket for HotKitchen, NOT for water
        tickets[0].KitchenStationId.Should().Be(fixture.hotStation.Id);
        tickets[0].IsHighPriority.Should().BeFalse();

        // --- Verify Invariant 3: Idempotency returns existing round without duplicate processing ---
        var duplicateResult = await pos.FireOrderAsync(fireRequest);
        duplicateResult.RoundId.Should().Be(roundDto.RoundId);
        duplicateResult.RoundNumber.Should().Be(roundDto.RoundNumber);

        // Verify total tickets in DB is still exactly 1 (no duplicate ticket created)
        var totalTicketsAfterDup = await db.KitchenTickets.CountAsync();
        totalTicketsAfterDup.Should().Be(1);

        // Verify reserved stock didn't double
        var beerInvAfterDup = await db.InventoryItems.FirstAsync(i => i.Id == fixture.beerInventory.Id);
        beerInvAfterDup.ReservedStock.Should().Be(4);

        // =========================================================================
        // STEP 4: Chef on KDS prepares and bumps the order ticket
        // =========================================================================
        var kdsTickets = await kds.GetActiveTicketsAsync(fixture.table.BranchId, fixture.hotStation.Id);
        kdsTickets.Should().HaveCount(1);
        kdsTickets[0].TableCode.Should().Be("VIP-01");
        kdsTickets[0].Items.Should().ContainSingle(i => i.ItemName == fixture.steakDish.Name && i.Quantity == 2);

        // Chef starts preparation -> status InPreparation
        await kds.UpdateTicketStatusAsync(new UpdateTicketStatusRequest(
            TicketId: kdsTickets[0].TicketId,
            NewStatus: KitchenTicketStatus.InPreparation,
            StaffId: _chefId
        ));

        var steakOrderItem = await db.OrderItems.FirstAsync(i => i.MenuItemId == fixture.steakDish.Id);
        steakOrderItem.Status.Should().Be(OrderItemStatus.Cooking);

        // Chef finishes cooking -> bumps ticket to Completed
        await kds.UpdateTicketStatusAsync(new UpdateTicketStatusRequest(
            TicketId: kdsTickets[0].TicketId,
            NewStatus: KitchenTicketStatus.Completed,
            StaffId: _chefId
        ));

        var steakAfterCook = await db.OrderItems.FirstAsync(i => i.MenuItemId == fixture.steakDish.Id);
        steakAfterCook.Status.Should().Be(OrderItemStatus.ReadyToServe);

        // Expediter pass marks dish as Served to table
        await kds.UpdateOrderItemStatusAsync(new UpdateItemStatusRequest(
            OrderItemId: steakAfterCook.Id,
            NewStatus: OrderItemStatus.Served,
            StaffId: _waiterId
        ));

        // =========================================================================
        // STEP 5: Waiter requests billing -> Table transitions to BillingRequested
        // =========================================================================
        await pos.RequestBillAsync(new RequestBillRequest(fixture.table.Id, _waiterId));

        var tableAfterBillingReq = await db.RestaurantTables.FirstAsync(t => t.Id == fixture.table.Id);
        tableAfterBillingReq.Status.Should().Be(TableStatus.BillingRequested);

        // =========================================================================
        // STEP 6: Cashier calculates bill with VIP Room Surcharges & VAT 8%
        // Subtotal = 2*500k (Steak) + 4*80k (Beer) + 2*35k (Water) = 1,000,000 + 320,000 + 70,000 = 1,390,000đ
        // Surcharge = 1,390,000 * 10% + 50,000 = 139,000 + 50,000 = 189,000đ (PDR-011)
        // Taxable = 1,390,000 + 189,000 = 1,579,000đ
        // VAT 8% = Round(1,579,000 * 8%) = 126,320đ
        // Total = 1,579,000 + 126,320 = 1,705,320đ
        // =========================================================================
        var order = await db.Orders.FirstAsync(o => o.DiningSessionId == openResult.SessionId);
        var billSummary = await cashier.GetBillCalculationAsync(order.Id);
        billSummary.Subtotal.Should().Be(1390000m);
        billSummary.SurchargeRate.Should().Be(10.0m);
        billSummary.SurchargeAmount.Should().Be(189000m);
        billSummary.TaxableAmount.Should().Be(1579000m);
        billSummary.VatRate.Should().Be(8.0m);
        billSummary.VatAmount.Should().Be(126320m);
        billSummary.TotalAmount.Should().Be(1705320m);

        // Create standard invoice
        var invoice = await cashier.CreateStandardInvoiceAsync(order.Id, _cashierId);
        invoice.TotalAmount.Should().Be(1705320m);
        invoice.IsSettled.Should().BeFalse();

        // Generate VietQR dynamic payload & validate EMVCo checksum
        var qrPayload = qrService.GenerateDynamicVietQr(invoice.InvoiceNumber, invoice.TotalAmount);
        qrPayload.Amount.Should().Be(1705320m);
        qrService.ValidateCrc16(qrPayload.QrString).Should().BeTrue();

        // =========================================================================
        // STEP 7: Cashier settles bill with Cash (2,000,000đ given -> 294,680đ change)
        // =========================================================================
        decimal cashGiven = 2000000m;
        decimal change = cashGiven - invoice.TotalAmount; // 294,680đ

        var settledInvoice = await cashier.SettleInvoiceAsync(new SettleInvoiceRequest(
            InvoiceId: invoice.InvoiceId,
            PaymentMethod: PaymentMethod.Cash,
            Amount: cashGiven,
            ChangeAmount: change,
            TransactionRef: "CASH-PAID",
            CashierStaffId: _cashierId
        ));

        settledInvoice.IsSettled.Should().BeTrue();
        settledInvoice.Status.Should().Be(InvoiceStatus.Paid);
        settledInvoice.PaidAmount.Should().Be(cashGiven);
        settledInvoice.ChangeAmount.Should().Be(change);

        // --- Verify Invariant 4: Cash added to active shift system cash total ---
        var updatedShift = await db.WorkShifts.FirstAsync(s => s.Id == shift.Id);
        updatedShift.SystemCashTotal.Should().Be(invoice.TotalAmount); // 1,705,320đ net cash

        // --- Verify Invariant 5: Table is automatically released to Available ---
        var tableAfterSettlement = await db.RestaurantTables.FirstAsync(t => t.Id == fixture.table.Id);
        tableAfterSettlement.Status.Should().Be(TableStatus.Available);

        // =========================================================================
        // STEP 8: Verify Settled Bill Immutability (OD-13)
        // =========================================================================
        var attemptDoubleSettlement = async () => await cashier.SettleInvoiceAsync(new SettleInvoiceRequest(
            InvoiceId: invoice.InvoiceId,
            PaymentMethod: PaymentMethod.Cash,
            Amount: 100000m,
            ChangeAmount: 0m,
            TransactionRef: null,
            CashierStaffId: _cashierId
        ));

        await attemptDoubleSettlement.Should().ThrowAsync<DomainValidationException>()
            .WithMessage("*OD-13*");

        // =========================================================================
        // STEP 9: Blind Shift Close with Variance Audit (OD-14 Option A & INV-12)
        // Opening Float: 1,000,000 + Revenue: 1,705,320 = Expected: 2,705,320đ
        // Cashier actual count: 2,700,000đ (Short by 5,320đ)
        // =========================================================================
        var closeResult = await cashier.CloseShiftBlindAsync(new BlindShiftCloseRequest(
            ShiftId: shift.Id,
            ActualCashCount: 2700000m,
            Notes: "Thiếu 5.320đ do khách làm rơi tiền lẻ",
            CashierStaffId: _cashierId
        ));

        closeResult.OpeningFloat.Should().Be(1000000m);
        closeResult.SystemCashTotal.Should().Be(1705320m);
        closeResult.ExpectedAmount.Should().Be(2705320m);
        closeResult.ActualCashCount.Should().Be(2700000m);
        closeResult.VarianceAmount.Should().Be(-5320m);
        closeResult.HasDiscrepancy.Should().BeTrue();

        var varianceLog = await db.ShiftClosingVarianceLogs.FirstOrDefaultAsync(l => l.ShiftId == shift.Id);
        varianceLog.Should().NotBeNull();
        varianceLog!.VarianceAmount.Should().Be(-5320m);
        varianceLog.CashierNotes.Should().Be("Thiếu 5.320đ do khách làm rơi tiền lẻ");

        var closedShiftEntity = await db.WorkShifts.FirstAsync(s => s.Id == shift.Id);
        closedShiftEntity.Status.Should().Be(ShiftStatus.Closed);
    }

    [Fact]
    public async Task SplitBill_ByItems_TableNotReleasedUntilFinalSubBillSettled_D4_PDR009()
    {
        var fixture = CreateIntegrationFixture(Guid.NewGuid().ToString());
        var db = fixture.db;
        var pos = fixture.posService;
        var cashier = fixture.cashierService;

        // Open session and order 2 Steaks
        var session = await pos.OpenDiningSessionAsync(new OpenDiningSessionRequest(fixture.table.Id, 2, _waiterId));
        var fireResult = await pos.FireOrderAsync(new FireOrderRequest(
            TableId: fixture.table.Id,
            OrderId: null,
            IdempotencyKey: Guid.NewGuid().ToString(),
            StaffId: _waiterId,
            Note: "Hai người ăn riêng",
            Items: new List<FireOrderItemDraftDto>
            {
                new(fixture.steakDish.Id, null, fixture.steakDish.Name, fixture.steakDish.BasePrice, 1, "Phần anh A", null),
                new(fixture.steakDish.Id, null, fixture.steakDish.Name, fixture.steakDish.BasePrice, 1, "Phần anh B", null)
            }
        ));

        var order = await db.Orders.FirstAsync(o => o.DiningSessionId == session.SessionId);
        var orderItems = await db.OrderItems.Where(i => i.OrderRoundId == fireResult.RoundId).ToListAsync();

        // Split bill into 2 sub-bills by items (Mode A)
        var splitInvoices = await cashier.SplitBillAsync(new SplitBillRequest(
            OrderId: order.Id,
            SplitMode: "by_items",
            EqualParts: null,
            Allocations: new List<List<SplitItemAllocation>>
            {
                new() { new(orderItems[0].Id, 1) },
                new() { new(orderItems[1].Id, 1) }
            },
            CashierStaffId: _cashierId
        ));

        splitInvoices.Should().HaveCount(2);

        // Settle Sub-Bill 1
        await cashier.SettleInvoiceAsync(new SettleInvoiceRequest(
            InvoiceId: splitInvoices[0].InvoiceId,
            PaymentMethod: PaymentMethod.PosCard,
            Amount: splitInvoices[0].TotalAmount,
            ChangeAmount: 0,
            TransactionRef: "CARD-SLIP-A",
            CashierStaffId: _cashierId
        ));

        // Invariant: Table must NOT be released yet because Sub-Bill 2 is unpaid!
        var tableAfterSubBill1 = await db.RestaurantTables.FirstAsync(t => t.Id == fixture.table.Id);
        tableAfterSubBill1.Status.Should().NotBe(TableStatus.Available);

        // Settle Sub-Bill 2
        await cashier.SettleInvoiceAsync(new SettleInvoiceRequest(
            InvoiceId: splitInvoices[1].InvoiceId,
            PaymentMethod: PaymentMethod.VietQR,
            Amount: splitInvoices[1].TotalAmount,
            ChangeAmount: 0,
            TransactionRef: "QR-TRANS-B",
            CashierStaffId: _cashierId
        ));

        // Now that 100% of sub-bills are settled, table is released!
        var tableAfterAllBills = await db.RestaurantTables.FirstAsync(t => t.Id == fixture.table.Id);
        tableAfterAllBills.Status.Should().Be(TableStatus.Available);
    }

    [Fact]
    public async Task RemakeDish_CulinaryMistake_ZeroCustomerCharge_WasteLossLogged_OD09()
    {
        var fixture = CreateIntegrationFixture(Guid.NewGuid().ToString());
        var db = fixture.db;
        var pos = fixture.posService;
        var kds = fixture.kitchenService;
        var cashier = fixture.cashierService;

        var session = await pos.OpenDiningSessionAsync(new OpenDiningSessionRequest(fixture.table.Id, 2, _waiterId));
        var fireResult = await pos.FireOrderAsync(new FireOrderRequest(
            TableId: fixture.table.Id,
            OrderId: null,
            IdempotencyKey: Guid.NewGuid().ToString(),
            StaffId: _waiterId,
            Note: "",
            Items: new List<FireOrderItemDraftDto>
            {
                new(fixture.steakDish.Id, null, fixture.steakDish.Name, fixture.steakDish.BasePrice, 1, "", null)
            }
        ));

        var orderItem = await db.OrderItems.FirstAsync(i => i.OrderRoundId == fireResult.RoundId);

        // Chef / Manager triggers Remake due to overcooked steak
        var remakeTicket = await kds.RemakeDishAsync(new RemakeDishRequest(
            OrderItemId: orderItem.Id,
            Reason: "Bò bị cháy xém cạnh do quá lửa",
            ManagerId: _managerId
        ));

        remakeTicket.Should().NotBeNull();
        remakeTicket.IsHighPriority.Should().BeTrue();
        remakeTicket.Items[0].IsRemake.Should().BeTrue();

        // Verify KitchenWasteLog created
        var wasteLog = await db.KitchenWasteLogs.FirstOrDefaultAsync(w => w.MenuItemId == fixture.steakDish.Id);
        wasteLog.Should().NotBeNull();
        wasteLog!.EstimatedLoss.Should().Be(fixture.steakDish.BasePrice);
        wasteLog.Reason.Should().Contain("LÀM LẠI MÓN LỖI BẾP");

        // Verify Bill Calculation: customer is NOT charged for the remade dish (0đ per OD-09 / REV-11)
        var order = await db.Orders.FirstAsync(o => o.DiningSessionId == session.SessionId);
        var bill = await cashier.GetBillCalculationAsync(order.Id);
        bill.Subtotal.Should().Be(0m); // Original item marked remake -> 0đ charge!
    }

    [Fact]
    public async Task DelayedVietQrWebhook_WhenAlreadySettled_LoggedToSuspenseLedger_PDR008()
    {
        var fixture = CreateIntegrationFixture(Guid.NewGuid().ToString());
        var db = fixture.db;
        var pos = fixture.posService;
        var cashier = fixture.cashierService;

        var session = await pos.OpenDiningSessionAsync(new OpenDiningSessionRequest(fixture.table.Id, 2, _waiterId));
        await pos.FireOrderAsync(new FireOrderRequest(
            TableId: fixture.table.Id,
            OrderId: null,
            IdempotencyKey: Guid.NewGuid().ToString(),
            StaffId: _waiterId,
            Note: "",
            Items: new List<FireOrderItemDraftDto>
            {
                new(fixture.steakDish.Id, null, fixture.steakDish.Name, fixture.steakDish.BasePrice, 1, "", null)
            }
        ));

        var order = await db.Orders.FirstAsync(o => o.DiningSessionId == session.SessionId);
        var invoice = await cashier.CreateStandardInvoiceAsync(order.Id, _cashierId);

        // Guest pays by Cash first
        await cashier.SettleInvoiceAsync(new SettleInvoiceRequest(
            InvoiceId: invoice.InvoiceId,
            PaymentMethod: PaymentMethod.Cash,
            Amount: invoice.TotalAmount,
            ChangeAmount: 0m,
            TransactionRef: "CASH-PAID",
            CashierStaffId: _cashierId
        ));

        // Delayed Bank Transfer Webhook arrives 10 minutes later (PDR-008)
        var webhookProcessed = await cashier.ProcessVietQrWebhookAsync(new ReconcileWebhookRequest(
            TransactionRef: "DELAYED-NAPAS-998877",
            Amount: invoice.TotalAmount,
            OrderCode: invoice.InvoiceNumber,
            BankCode: "MB",
            PayerInfo: "KHACH HANG CHUYEN THUA"
        ));

        webhookProcessed.Should().BeTrue();

        // Must be recorded in SuspenseLedger with state Unreconciled
        var suspenseItem = await db.SuspenseLedgers.FirstOrDefaultAsync(s => s.TransactionRef == "DELAYED-NAPAS-998877");
        suspenseItem.Should().NotBeNull();
        suspenseItem!.Status.Should().Be(SuspenseLedgerStatus.Unreconciled);
        suspenseItem.Amount.Should().Be(invoice.TotalAmount);
        suspenseItem.Reason.Should().Contain("trước đó");

        // Manager resolves the suspense item
        suspenseItem.Resolve(_managerId, "Đã liên hệ khách và chuyển khoản hoàn tiền thành công");
        await db.SaveChangesAsync();

        suspenseItem.Status.Should().Be(SuspenseLedgerStatus.Resolved);
        suspenseItem.ResolvedByStaffId.Should().Be(_managerId);
    }
}
