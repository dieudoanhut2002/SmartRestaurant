namespace SmartRestaurant.Domain.Entities;

using SmartRestaurant.Domain.Common;
using SmartRestaurant.Domain.Enums;
using SmartRestaurant.Domain.Exceptions;

public enum PaymentTransactionStatus
{
    Pending = 1,
    Success = 2,
    Failed = 3
}

public class PaymentTransaction : BaseEntity
{
    public Guid InvoiceId { get; private set; }
    public Guid BranchId { get; private set; }
    public PaymentMethod PaymentMethod { get; private set; }
    public decimal Amount { get; private set; }
    public PaymentTransactionStatus Status { get; private set; } = PaymentTransactionStatus.Pending;
    public string? TransactionRef { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    protected PaymentTransaction() { }

    public PaymentTransaction(
        Guid invoiceId,
        Guid branchId,
        PaymentMethod paymentMethod,
        decimal amount,
        PaymentTransactionStatus status = PaymentTransactionStatus.Success,
        string? transactionRef = null)
    {
        if (invoiceId == Guid.Empty)
            throw new DomainValidationException("InvoiceId không được để trống.");
        if (amount <= 0)
            throw new DomainValidationException("Số tiền giao dịch phải lớn hơn 0.");

        InvoiceId = invoiceId;
        BranchId = branchId;
        PaymentMethod = paymentMethod;
        Amount = amount;
        Status = status;
        TransactionRef = transactionRef?.Trim();
        if (status == PaymentTransactionStatus.Success)
        {
            CompletedAt = DateTime.UtcNow;
        }
    }
}
