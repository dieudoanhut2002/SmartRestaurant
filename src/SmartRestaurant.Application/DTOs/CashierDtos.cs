namespace SmartRestaurant.Application.DTOs;

using SmartRestaurant.Domain.Enums;

public record CashierTableItemDto(
    Guid ItemId,
    Guid MenuItemId,
    string ItemName,
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice,
    string? Notes,
    OrderItemStatus Status,
    List<string> Modifiers);

public record CashierBillSummaryDto(
    Guid SessionId,
    Guid OrderId,
    string TableCode,
    string TableName,
    string AreaName,
    DateTime StartedAt,
    decimal Subtotal,
    decimal SurchargeAmount,
    decimal SurchargeRate,
    decimal DiscountAmount,
    decimal TaxableAmount,
    decimal VatRate,
    decimal VatAmount,
    decimal TotalAmount,
    List<CashierTableItemDto> Items);

public record SplitItemAllocation(
    Guid OrderItemId,
    int Quantity);

public record SplitBillRequest(
    Guid OrderId,
    string SplitMode, // "by_items" | "equal"
    int? EqualParts,
    List<List<SplitItemAllocation>>? Allocations,
    Guid CashierStaffId);

public record InvoiceDto(
    Guid InvoiceId,
    string InvoiceNumber,
    Guid OrderId,
    Guid DiningSessionId,
    string TableCode,
    decimal Subtotal,
    decimal SurchargeAmount,
    decimal DiscountAmount,
    decimal VatRate,
    decimal VatAmount,
    decimal TotalAmount,
    decimal PaidAmount,
    decimal ChangeAmount,
    InvoiceStatus Status,
    PaymentMethod PaymentMethod,
    bool IsSettled,
    DateTime? SettledAt,
    List<CashierTableItemDto> Items);

public record SettleInvoiceRequest(
    Guid InvoiceId,
    PaymentMethod PaymentMethod,
    decimal Amount,
    decimal ChangeAmount,
    string? TransactionRef,
    Guid CashierStaffId);

public record VietQrPayloadDto(
    string QrString,
    string BankBin,
    string BankName,
    string AccountNumber,
    string AccountName,
    decimal Amount,
    string OrderCode,
    string QuickLink);

public record ReconcileWebhookRequest(
    string TransactionRef,
    decimal Amount,
    string OrderCode,
    string? BankCode,
    string? PayerInfo);

public record BlindShiftCloseRequest(
    Guid ShiftId,
    decimal ActualCashCount,
    string Notes,
    Guid CashierStaffId);

public record ShiftCloseResultDto(
    Guid ShiftId,
    decimal OpeningFloat,
    decimal SystemCashTotal,
    decimal ActualCashCount,
    decimal ExpectedAmount,
    decimal VarianceAmount,
    bool HasDiscrepancy,
    string Notes);

public record SuspenseLedgerItemDto(
    Guid Id,
    Guid BranchId,
    decimal Amount,
    string TransactionRef,
    string? BankCode,
    string? PayerInfo,
    string Reason,
    SuspenseLedgerStatus Status,
    DateTime ReceivedAt,
    DateTime? ResolvedAt,
    string? ResolutionNotes);
