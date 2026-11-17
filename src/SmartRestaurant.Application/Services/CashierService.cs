namespace SmartRestaurant.Application.Services;

using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SmartRestaurant.Application.Common.Interfaces;
using SmartRestaurant.Application.DTOs;
using SmartRestaurant.Application.Interfaces;
using SmartRestaurant.Domain.Entities;
using SmartRestaurant.Domain.Enums;
using SmartRestaurant.Domain.Exceptions;

public class CashierService(IAppDbContext dbContext) : ICashierService
{
    private readonly IAppDbContext _dbContext = dbContext;
    private const decimal DefaultVatRate = 8.0m; // 8% VAT per standard F&B tax reduction

    public async Task<IReadOnlyList<CashierBillSummaryDto>> GetActiveBillingTablesAsync(Guid branchId, CancellationToken ct = default)
    {
        var tables = await _dbContext.RestaurantTables
            .Where(t => t.BranchId == branchId && (t.Status == TableStatus.BillingRequested || t.Status == TableStatus.Occupied))
            .OrderBy(t => t.Code)
            .ToListAsync(ct);

        var list = new List<CashierBillSummaryDto>();

        foreach (var tbl in tables)
        {
            var session = await _dbContext.DiningSessions
                .Where(s => s.TableId == tbl.Id && s.Status == DiningSessionStatus.Active)
                .OrderByDescending(s => s.StartedAt)
                .FirstOrDefaultAsync(ct);

            if (session == null) continue;

            var order = await _dbContext.Orders
                .Where(o => o.DiningSessionId == session.Id && !o.IsSettled)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync(ct);

            if (order == null) continue;

            var summary = await GetBillCalculationAsync(order.Id, ct);
            list.Add(summary);
        }

        return list;
    }

    public async Task<CashierBillSummaryDto> GetBillCalculationAsync(Guid orderId, CancellationToken ct = default)
    {
        var order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.Id == orderId, ct)
            ?? throw new DomainNotFoundException("Không tìm thấy đơn hàng yêu cầu tính tiền.");

        var session = await _dbContext.DiningSessions.FirstOrDefaultAsync(s => s.Id == order.DiningSessionId, ct)
            ?? throw new DomainNotFoundException("Không tìm thấy phiên dùng bữa của bàn.");

        var table = await _dbContext.RestaurantTables.FirstOrDefaultAsync(t => t.Id == session.TableId, ct)
            ?? throw new DomainNotFoundException("Không tìm thấy thông tin bàn ăn.");

        var area = await _dbContext.DiningAreas.FirstOrDefaultAsync(a => a.Id == table.DiningAreaId, ct);
        var room = table.DiningRoomId.HasValue
            ? await _dbContext.DiningRooms.FirstOrDefaultAsync(r => r.Id == table.DiningRoomId.Value, ct)
            : null;

        var rounds = await _dbContext.OrderRounds
            .Where(r => r.OrderId == order.Id)
            .ToListAsync(ct);

        var roundIds = rounds.Select(r => r.Id).ToList();

        var items = await _dbContext.OrderItems
            .Include(i => i.Modifiers)
            .Where(i => roundIds.Contains(i.OrderRoundId) && i.Status != OrderItemStatus.Cancelled)
            .ToListAsync(ct);

        var itemDtos = new List<CashierTableItemDto>();
        decimal subtotal = 0;

        foreach (var i in items)
        {
            decimal modTotal = i.Modifiers.Sum(m => m.PriceAdjustment);
            decimal itemEffectiveUnit = i.IsRemake ? 0 : (i.UnitPrice + modTotal); // 0đ for remake per OD-09
            decimal rowTotal = itemEffectiveUnit * i.Quantity;
            subtotal += rowTotal;

            itemDtos.Add(new CashierTableItemDto(
                ItemId: i.Id,
                MenuItemId: i.MenuItemId,
                ItemName: i.ItemName,
                Quantity: i.Quantity,
                UnitPrice: i.UnitPrice + modTotal,
                TotalPrice: rowTotal,
                Notes: i.KitchenNote,
                Status: i.Status,
                Modifiers: i.Modifiers.Select(m => m.OptionName).ToList()
            ));
        }

        // Calculate VIP / Room Surcharges per PDR-011
        decimal surchargeRate = room?.SurchargeRate ?? 0;
        decimal surchargeFixed = room?.SurchargeFixedAmount ?? 0;
        decimal surchargeAmount = Math.Round((subtotal * (surchargeRate / 100m)) + surchargeFixed, 0, MidpointRounding.AwayFromZero);

        decimal discountAmount = order.DiscountAmount;
        decimal taxable = Math.Max(0, subtotal + surchargeAmount - discountAmount);
        decimal vatRate = DefaultVatRate;
        decimal vatAmount = Math.Round(taxable * (vatRate / 100m), 0, MidpointRounding.AwayFromZero);
        decimal totalAmount = taxable + vatAmount;

        return new CashierBillSummaryDto(
            SessionId: session.Id,
            OrderId: order.Id,
            TableCode: table.Code,
            TableName: table.Name,
            AreaName: area?.Name ?? "Khu Vực",
            StartedAt: session.StartedAt,
            Subtotal: subtotal,
            SurchargeAmount: surchargeAmount,
            SurchargeRate: surchargeRate,
            DiscountAmount: discountAmount,
            TaxableAmount: taxable,
            VatRate: vatRate,
            VatAmount: vatAmount,
            TotalAmount: totalAmount,
            Items: itemDtos
        );
    }

    public async Task<InvoiceDto> CreateStandardInvoiceAsync(Guid orderId, Guid cashierStaffId, CancellationToken ct = default)
    {
        // Check if an open pending invoice already exists
        var existingInvoice = await _dbContext.Invoices
            .Include(i => i.Transactions)
            .FirstOrDefaultAsync(i => i.OrderId == orderId && !i.IsSettled && i.Status == InvoiceStatus.Pending, ct);

        if (existingInvoice != null)
        {
            return MapToInvoiceDto(existingInvoice);
        }

        var calc = await GetBillCalculationAsync(orderId, ct);
        var order = await _dbContext.Orders.FirstAsync(o => o.Id == orderId, ct);

        var shift = await _dbContext.WorkShifts
            .Where(s => s.BranchId == order.BranchId && s.Status == ShiftStatus.Open)
            .OrderByDescending(s => s.OpenedAt)
            .FirstOrDefaultAsync(ct);

        string invoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{calc.TableCode}-{DateTime.UtcNow:HHmmss}";
        string itemsJson = JsonSerializer.Serialize(calc.Items);

        var invoice = new Invoice(
            branchId: order.BranchId,
            diningSessionId: calc.SessionId,
            orderId: order.Id,
            cashierStaffId: cashierStaffId,
            invoiceNumber: invoiceNumber,
            subtotal: calc.Subtotal,
            surchargeAmount: calc.SurchargeAmount,
            discountAmount: calc.DiscountAmount,
            vatRate: calc.VatRate,
            itemsJson: itemsJson,
            workShiftId: shift?.Id
        );

        _dbContext.Invoices.Add(invoice);
        await _dbContext.SaveChangesAsync(ct);

        return MapToInvoiceDto(invoice, calc.TableCode, calc.Items);
    }

    public async Task<IReadOnlyList<InvoiceDto>> SplitBillAsync(SplitBillRequest request, CancellationToken ct = default)
    {
        var calc = await GetBillCalculationAsync(request.OrderId, ct);
        var order = await _dbContext.Orders.FirstAsync(o => o.Id == request.OrderId, ct);

        var shift = await _dbContext.WorkShifts
            .Where(s => s.BranchId == order.BranchId && s.Status == ShiftStatus.Open)
            .OrderByDescending(s => s.OpenedAt)
            .FirstOrDefaultAsync(ct);

        var createdInvoices = new List<InvoiceDto>();

        if (string.Equals(request.SplitMode, "equal", StringComparison.OrdinalIgnoreCase) && request.EqualParts.HasValue && request.EqualParts.Value >= 2)
        {
            // Mode B: Equal Split
            int parts = request.EqualParts.Value;
            decimal subtotalPart = Math.Round(calc.Subtotal / parts, 0);
            decimal surchargePart = Math.Round(calc.SurchargeAmount / parts, 0);
            decimal discountPart = Math.Round(calc.DiscountAmount / parts, 0);

            for (int i = 1; i <= parts; i++)
            {
                string invNum = $"INV-{DateTime.UtcNow:yyyyMMdd}-{calc.TableCode}-P{i}";
                var invoice = new Invoice(
                    branchId: order.BranchId,
                    diningSessionId: calc.SessionId,
                    orderId: order.Id,
                    cashierStaffId: request.CashierStaffId,
                    invoiceNumber: invNum,
                    subtotal: subtotalPart,
                    surchargeAmount: surchargePart,
                    discountAmount: discountPart,
                    vatRate: calc.VatRate,
                    itemsJson: $"[Tách đều 1/{parts} toàn bộ bàn]",
                    workShiftId: shift?.Id,
                    notes: $"Hóa đơn tách đều phần {i}/{parts}"
                );
                _dbContext.Invoices.Add(invoice);
                await _dbContext.SaveChangesAsync(ct);
                createdInvoices.Add(MapToInvoiceDto(invoice, calc.TableCode, new()));
            }
        }
        else if (request.Allocations != null && request.Allocations.Count >= 2)
        {
            // Mode A: Split by selected items
            int subIndex = 1;
            foreach (var allocGroup in request.Allocations)
            {
                var allocatedItemDtos = new List<CashierTableItemDto>();
                decimal groupSubtotal = 0;

                foreach (var alloc in allocGroup)
                {
                    var sourceItem = calc.Items.FirstOrDefault(it => it.ItemId == alloc.OrderItemId);
                    if (sourceItem != null)
                    {
                        decimal itemTotal = sourceItem.UnitPrice * alloc.Quantity;
                        groupSubtotal += itemTotal;
                        allocatedItemDtos.Add(sourceItem with { Quantity = alloc.Quantity, TotalPrice = itemTotal });
                    }
                }

                decimal ratio = calc.Subtotal > 0 ? (groupSubtotal / calc.Subtotal) : 0;
                decimal groupSurcharge = Math.Round(calc.SurchargeAmount * ratio, 0);
                decimal groupDiscount = Math.Round(calc.DiscountAmount * ratio, 0);

                string invNum = $"INV-{DateTime.UtcNow:yyyyMMdd}-{calc.TableCode}-SPLIT{subIndex}";
                var invoice = new Invoice(
                    branchId: order.BranchId,
                    diningSessionId: calc.SessionId,
                    orderId: order.Id,
                    cashierStaffId: request.CashierStaffId,
                    invoiceNumber: invNum,
                    subtotal: groupSubtotal,
                    surchargeAmount: groupSurcharge,
                    discountAmount: groupDiscount,
                    vatRate: calc.VatRate,
                    itemsJson: JsonSerializer.Serialize(allocatedItemDtos),
                    workShiftId: shift?.Id,
                    notes: $"Hóa đơn tách món phần {subIndex}"
                );

                _dbContext.Invoices.Add(invoice);
                await _dbContext.SaveChangesAsync(ct);
                createdInvoices.Add(MapToInvoiceDto(invoice, calc.TableCode, allocatedItemDtos));
                subIndex++;
            }
        }
        else
        {
            throw new DomainValidationException("Cấu hình tách hóa đơn không hợp lệ.");
        }

        return createdInvoices;
    }

    public async Task<InvoiceDto> SettleInvoiceAsync(SettleInvoiceRequest request, CancellationToken ct = default)
    {
        var invoice = await _dbContext.Invoices
            .Include(i => i.Transactions)
            .FirstOrDefaultAsync(i => i.Id == request.InvoiceId, ct)
            ?? throw new DomainNotFoundException("Không tìm thấy hóa đơn cần thanh toán.");

        if (invoice.IsSettled)
            throw new DomainValidationException("Hóa đơn đã được quyết toán trước đó (OD-13).");

        var tx = invoice.ApplyPayment(request.Amount, request.PaymentMethod, request.ChangeAmount, request.TransactionRef);
        _dbContext.PaymentTransactions.Add(tx);

        // If Cash payment, record in active work shift
        if (request.PaymentMethod == PaymentMethod.Cash && invoice.WorkShiftId.HasValue)
        {
            var shift = await _dbContext.WorkShifts.FirstOrDefaultAsync(s => s.Id == invoice.WorkShiftId.Value, ct);
            if (shift != null && shift.Status == ShiftStatus.Open)
            {
                decimal netCash = Math.Max(0, request.Amount - request.ChangeAmount);
                shift.AddSystemCash(netCash);
            }
        }

        // Check if ALL invoices for this DiningSession are now settled (D4 / PDR-009 §4)
        var allInvoicesForSession = await _dbContext.Invoices
            .Where(i => i.DiningSessionId == invoice.DiningSessionId && i.Status != InvoiceStatus.Cancelled)
            .ToListAsync(ct);

        bool allSettled = allInvoicesForSession.All(i => i.IsSettled);

        if (allSettled)
        {
            var session = await _dbContext.DiningSessions.FirstOrDefaultAsync(s => s.Id == invoice.DiningSessionId, ct);
            if (session != null)
            {
                var table = await _dbContext.RestaurantTables.FirstOrDefaultAsync(t => t.Id == session.TableId, ct);
                if (table != null)
                {
                    table.ResetToAvailable();

                    _dbContext.OutboxMessages.Add(new OutboxMessage(
                        "TableStatusChanged",
                        JsonSerializer.Serialize(new
                        {
                            TableId = table.Id,
                            TableCode = table.Code,
                            NewStatus = TableStatus.Available,
                            Reason = "Hóa đơn đã thanh toán hoàn tất (Bàn trống sẵn sàng đón khách mới)"
                        })
                    ));
                }
            }

            var order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.Id == invoice.OrderId, ct);
            if (order != null && !order.IsSettled)
            {
                // Settle order
                var rounds = await _dbContext.OrderRounds.Where(r => r.OrderId == order.Id).ToListAsync(ct);
                // Mark order settled in domain
            }
        }

        _dbContext.OutboxMessages.Add(new OutboxMessage(
            "InvoiceSettled",
            JsonSerializer.Serialize(new
            {
                InvoiceId = invoice.Id,
                InvoiceNumber = invoice.InvoiceNumber,
                TotalAmount = invoice.TotalAmount,
                PaymentMethod = invoice.PaymentMethod,
                AllBillsSettled = allSettled
            })
        ));

        await _dbContext.SaveChangesAsync(ct);
        return MapToInvoiceDto(invoice);
    }

    public async Task<bool> ProcessVietQrWebhookAsync(ReconcileWebhookRequest request, CancellationToken ct = default)
    {
        string refCode = request.OrderCode.Replace("-", "").Trim().ToUpperInvariant();

        // 1. Try to find matching Invoice by InvoiceNumber
        var invoice = await _dbContext.Invoices
            .FirstOrDefaultAsync(i => i.InvoiceNumber.Replace("-", "") == refCode, ct);

        if (invoice == null)
        {
            // Try matching by OrderCode
            var order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.OrderCode.Replace("-", "") == refCode, ct);
            if (order != null)
            {
                invoice = await _dbContext.Invoices.FirstOrDefaultAsync(i => i.OrderId == order.Id && !i.IsSettled, ct);
            }
        }

        var branch = await _dbContext.Branches.FirstOrDefaultAsync(ct);
        var branchId = invoice?.BranchId ?? branch?.Id ?? (await _dbContext.Orders.Select(o => o.BranchId).FirstOrDefaultAsync(ct));

        if (invoice != null)
        {
            if (!invoice.IsSettled)
            {
                // Auto-settle invoice
                await SettleInvoiceAsync(new SettleInvoiceRequest(
                    InvoiceId: invoice.Id,
                    PaymentMethod: PaymentMethod.VietQR,
                    Amount: request.Amount,
                    ChangeAmount: 0,
                    TransactionRef: request.TransactionRef,
                    CashierStaffId: invoice.CashierStaffId
                ), ct);
                return true;
            }
            else
            {
                // PDR-008: Bill was already settled by Cash before transfer webhook arrived -> Suspense Ledger
                _dbContext.SuspenseLedgers.Add(new SuspenseLedger(
                    branchId: branchId,
                    paymentMethod: PaymentMethod.VietQR,
                    amount: request.Amount,
                    transactionRef: request.TransactionRef,
                    reason: $"Hóa đơn {invoice.InvoiceNumber} đã được thanh toán hoàn tất trước đó (Thừa tiền chuyển khoản)",
                    invoiceId: invoice.Id,
                    bankCode: request.BankCode,
                    payerInfo: request.PayerInfo
                ));
                await _dbContext.SaveChangesAsync(ct);
                return true;
            }
        }

        // Invoice not found -> Suspense Ledger per PDR-008
        _dbContext.SuspenseLedgers.Add(new SuspenseLedger(
            branchId: branchId,
            paymentMethod: PaymentMethod.VietQR,
            amount: request.Amount,
            transactionRef: request.TransactionRef,
            reason: $"Không tìm thấy hóa đơn hoặc đơn hàng tương ứng với mã thanh toán '{request.OrderCode}'",
            bankCode: request.BankCode,
            payerInfo: request.PayerInfo
        ));

        await _dbContext.SaveChangesAsync(ct);
        return true;
    }

    public async Task<ShiftCloseResultDto> CloseShiftBlindAsync(BlindShiftCloseRequest request, CancellationToken ct = default)
    {
        var shift = await _dbContext.WorkShifts.FirstOrDefaultAsync(s => s.Id == request.ShiftId, ct)
            ?? throw new DomainNotFoundException("Không tìm thấy ca làm việc cần đóng.");

        if (shift.Status != ShiftStatus.Open)
            throw new DomainValidationException("Ca làm việc này đã được kết thúc trước đó.");

        // Calculate expected amount
        decimal expected = shift.OpeningCashFloat + shift.SystemCashTotal;
        decimal variance = request.ActualCashCount - expected;

        // Perform blind shift close in domain
        shift.CloseShiftBlind(request.ActualCashCount, shift.SystemCashTotal, request.Notes);

        // Record audit variance log
        _dbContext.ShiftClosingVarianceLogs.Add(new ShiftClosingVarianceLog(
            shiftId: shift.Id,
            branchId: shift.BranchId,
            openingFloat: shift.OpeningCashFloat,
            systemRevenue: shift.SystemCashTotal,
            actualCountedAmount: request.ActualCashCount,
            cashierNotes: request.Notes
        ));

        await _dbContext.SaveChangesAsync(ct);

        return new ShiftCloseResultDto(
            ShiftId: shift.Id,
            OpeningFloat: shift.OpeningCashFloat,
            SystemCashTotal: shift.SystemCashTotal,
            ActualCashCount: request.ActualCashCount,
            ExpectedAmount: expected,
            VarianceAmount: variance,
            HasDiscrepancy: variance != 0,
            Notes: request.Notes
        );
    }

    public async Task<IReadOnlyList<SuspenseLedgerItemDto>> GetActiveSuspenseItemsAsync(Guid branchId, CancellationToken ct = default)
    {
        var list = await _dbContext.SuspenseLedgers
            .Where(s => s.BranchId == branchId && s.Status == SuspenseLedgerStatus.Unreconciled)
            .OrderByDescending(s => s.ReceivedAt)
            .ToListAsync(ct);

        return list.Select(s => new SuspenseLedgerItemDto(
            Id: s.Id,
            BranchId: s.BranchId,
            Amount: s.Amount,
            TransactionRef: s.TransactionRef,
            BankCode: s.BankCode,
            PayerInfo: s.PayerInfo,
            Reason: s.Reason,
            Status: s.Status,
            ReceivedAt: s.ReceivedAt,
            ResolvedAt: s.ResolvedAt,
            ResolutionNotes: s.ResolutionNotes
        )).ToList();
    }

    public async Task<InvoiceDto> GetInvoiceByIdAsync(Guid invoiceId, CancellationToken ct = default)
    {
        var invoice = await _dbContext.Invoices
            .Include(i => i.Transactions)
            .FirstOrDefaultAsync(i => i.Id == invoiceId, ct)
            ?? throw new DomainNotFoundException("Không tìm thấy hóa đơn yêu cầu.");

        return MapToInvoiceDto(invoice);
    }

    private static InvoiceDto MapToInvoiceDto(Invoice inv, string tableCode = "", List<CashierTableItemDto>? items = null)
    {
        List<CashierTableItemDto> itemList = items ?? new();
        if (itemList.Count == 0 && !string.IsNullOrWhiteSpace(inv.ItemsJson) && inv.ItemsJson.StartsWith("["))
        {
            try
            {
                itemList = JsonSerializer.Deserialize<List<CashierTableItemDto>>(inv.ItemsJson) ?? new();
            }
            catch
            {
                // Fallback empty list
            }
        }

        return new InvoiceDto(
            InvoiceId: inv.Id,
            InvoiceNumber: inv.InvoiceNumber,
            OrderId: inv.OrderId,
            DiningSessionId: inv.DiningSessionId,
            TableCode: string.IsNullOrEmpty(tableCode) ? "BÀN" : tableCode,
            Subtotal: inv.Subtotal,
            SurchargeAmount: inv.SurchargeAmount,
            DiscountAmount: inv.DiscountAmount,
            VatRate: inv.VatRate,
            VatAmount: inv.VatAmount,
            TotalAmount: inv.TotalAmount,
            PaidAmount: inv.PaidAmount,
            ChangeAmount: inv.ChangeAmount,
            Status: inv.Status,
            PaymentMethod: inv.PaymentMethod,
            IsSettled: inv.IsSettled,
            SettledAt: inv.SettledAt,
            Items: itemList
        );
    }
}
