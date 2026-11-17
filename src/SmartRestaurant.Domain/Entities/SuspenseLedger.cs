namespace SmartRestaurant.Domain.Entities;

using SmartRestaurant.Domain.Common;
using SmartRestaurant.Domain.Enums;
using SmartRestaurant.Domain.Exceptions;

public class SuspenseLedger : BaseEntity, IAggregateRoot
{
    public Guid BranchId { get; private set; }
    public Guid? InvoiceId { get; private set; }
    public PaymentMethod PaymentMethod { get; private set; }
    public decimal Amount { get; private set; }
    public string TransactionRef { get; private set; } = string.Empty;
    public string? BankCode { get; private set; }
    public string? PayerInfo { get; private set; }
    public string Reason { get; private set; } = string.Empty;
    public SuspenseLedgerStatus Status { get; private set; } = SuspenseLedgerStatus.Unreconciled;
    public DateTime ReceivedAt { get; private set; }
    public DateTime? ResolvedAt { get; private set; }
    public Guid? ResolvedByStaffId { get; private set; }
    public string? ResolutionNotes { get; private set; }

    protected SuspenseLedger() { }

    public SuspenseLedger(
        Guid branchId,
        PaymentMethod paymentMethod,
        decimal amount,
        string transactionRef,
        string reason,
        Guid? invoiceId = null,
        string? bankCode = null,
        string? payerInfo = null)
    {
        if (branchId == Guid.Empty)
            throw new DomainValidationException("BranchId không được để trống.");
        if (amount <= 0)
            throw new DomainValidationException("Số tiền giao dịch treo phải lớn hơn 0.");
        if (string.IsNullOrWhiteSpace(transactionRef))
            throw new DomainValidationException("Mã tham chiếu giao dịch không được để trống.");

        BranchId = branchId;
        InvoiceId = invoiceId;
        PaymentMethod = paymentMethod;
        Amount = amount;
        TransactionRef = transactionRef.Trim();
        Reason = reason?.Trim() ?? "Giao dịch không khớp trạng thái đơn hàng hiện tại.";
        BankCode = bankCode?.Trim();
        PayerInfo = payerInfo?.Trim();
        Status = SuspenseLedgerStatus.Unreconciled;
        ReceivedAt = DateTime.UtcNow;
    }

    public void Resolve(Guid staffMemberId, string notes)
    {
        if (Status != SuspenseLedgerStatus.Unreconciled)
            throw new DomainValidationException("Khoản tiền treo này đã được xử lý trước đó.");

        Status = SuspenseLedgerStatus.Resolved;
        ResolvedByStaffId = staffMemberId;
        ResolutionNotes = notes?.Trim();
        ResolvedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Refund(Guid staffMemberId, string notes)
    {
        if (Status != SuspenseLedgerStatus.Unreconciled)
            throw new DomainValidationException("Khoản tiền treo này đã được xử lý trước đó.");

        Status = SuspenseLedgerStatus.Refunded;
        ResolvedByStaffId = staffMemberId;
        ResolutionNotes = notes?.Trim();
        ResolvedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
