namespace SmartRestaurant.Application.Interfaces;

using SmartRestaurant.Application.DTOs;

public interface ICashierService
{
    Task<IReadOnlyList<CashierBillSummaryDto>> GetActiveBillingTablesAsync(Guid branchId, CancellationToken ct = default);

    Task<CashierBillSummaryDto> GetBillCalculationAsync(Guid orderId, CancellationToken ct = default);

    Task<InvoiceDto> CreateStandardInvoiceAsync(Guid orderId, Guid cashierStaffId, CancellationToken ct = default);

    Task<IReadOnlyList<InvoiceDto>> SplitBillAsync(SplitBillRequest request, CancellationToken ct = default);

    Task<InvoiceDto> SettleInvoiceAsync(SettleInvoiceRequest request, CancellationToken ct = default);

    Task<bool> ProcessVietQrWebhookAsync(ReconcileWebhookRequest request, CancellationToken ct = default);

    Task<ShiftCloseResultDto> CloseShiftBlindAsync(BlindShiftCloseRequest request, CancellationToken ct = default);

    Task<IReadOnlyList<SuspenseLedgerItemDto>> GetActiveSuspenseItemsAsync(Guid branchId, CancellationToken ct = default);

    Task<InvoiceDto> GetInvoiceByIdAsync(Guid invoiceId, CancellationToken ct = default);
}
