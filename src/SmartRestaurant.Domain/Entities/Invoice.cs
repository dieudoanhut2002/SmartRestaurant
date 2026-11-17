namespace SmartRestaurant.Domain.Entities;

using SmartRestaurant.Domain.Common;
using SmartRestaurant.Domain.Enums;
using SmartRestaurant.Domain.Exceptions;

public class Invoice : BaseEntity, IAggregateRoot
{
    private readonly List<PaymentTransaction> _transactions = [];

    public Guid BranchId { get; private set; }
    public Guid DiningSessionId { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid? WorkShiftId { get; private set; }
    public Guid CashierStaffId { get; private set; }
    public string InvoiceNumber { get; private set; } = string.Empty;
    public string? CustomerName { get; private set; }
    public string? CustomerPhone { get; private set; }

    public decimal Subtotal { get; private set; }
    public decimal SurchargeAmount { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal TaxableAmount { get; private set; }
    public decimal VatRate { get; private set; } // e.g. 8 or 10
    public decimal VatAmount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public decimal PaidAmount { get; private set; }
    public decimal ChangeAmount { get; private set; }

    public InvoiceStatus Status { get; private set; } = InvoiceStatus.Pending;
    public PaymentMethod PaymentMethod { get; private set; } = PaymentMethod.Cash;
    public bool IsSettled { get; private set; }
    public DateTime? SettledAt { get; private set; }
    public string ItemsJson { get; private set; } = "[]";
    public string? Notes { get; private set; }

    public IReadOnlyCollection<PaymentTransaction> Transactions => _transactions.AsReadOnly();

    protected Invoice() { }

    public Invoice(
        Guid branchId,
        Guid diningSessionId,
        Guid orderId,
        Guid cashierStaffId,
        string invoiceNumber,
        decimal subtotal,
        decimal surchargeAmount,
        decimal discountAmount,
        decimal vatRate,
        string itemsJson,
        Guid? workShiftId = null,
        string? customerName = null,
        string? customerPhone = null,
        string? notes = null)
    {
        if (branchId == Guid.Empty)
            throw new DomainValidationException("BranchId không được để trống.");
        if (diningSessionId == Guid.Empty)
            throw new DomainValidationException("DiningSessionId không được để trống.");
        if (orderId == Guid.Empty)
            throw new DomainValidationException("OrderId không được để trống.");
        if (string.IsNullOrWhiteSpace(invoiceNumber))
            throw new DomainValidationException("Số hóa đơn không được để trống.");
        if (subtotal < 0)
            throw new DomainValidationException("Tiền hàng không được âm.");
        if (vatRate < 0 || vatRate > 100)
            throw new DomainValidationException("Thuế suất VAT không hợp lệ.");

        BranchId = branchId;
        DiningSessionId = diningSessionId;
        OrderId = orderId;
        CashierStaffId = cashierStaffId;
        WorkShiftId = workShiftId;
        InvoiceNumber = invoiceNumber.Trim().ToUpperInvariant();
        CustomerName = customerName?.Trim();
        CustomerPhone = customerPhone?.Trim();
        ItemsJson = string.IsNullOrWhiteSpace(itemsJson) ? "[]" : itemsJson.Trim();
        Notes = notes?.Trim();

        Subtotal = subtotal;
        SurchargeAmount = Math.Max(0, surchargeAmount);
        DiscountAmount = Math.Max(0, discountAmount);
        TaxableAmount = Math.Max(0, Subtotal + SurchargeAmount - DiscountAmount);
        VatRate = vatRate;
        VatAmount = Math.Round(TaxableAmount * (vatRate / 100m), 0, MidpointRounding.AwayFromZero);
        TotalAmount = TaxableAmount + VatAmount;

        PaidAmount = 0;
        ChangeAmount = 0;
        Status = InvoiceStatus.Pending;
        IsSettled = false;
    }

    public PaymentTransaction ApplyPayment(decimal amount, PaymentMethod method, decimal changeAmount = 0, string? transactionRef = null)
    {
        // OD-13: Settled bills are strictly immutable
        if (IsSettled)
            throw new DomainValidationException("Hóa đơn đã được quyết toán hoàn tất (Paid), không thể sửa đổi hoặc thanh toán thêm. (OD-13)");

        if (amount <= 0)
            throw new DomainValidationException("Số tiền thanh toán phải lớn hơn 0.");

        var tx = new PaymentTransaction(Id, BranchId, method, amount, PaymentTransactionStatus.Success, transactionRef);
        if (!_transactions.Any(t => t.Id == tx.Id))
        {
            _transactions.Add(tx);
        }

        PaidAmount += amount;
        ChangeAmount = Math.Max(0, changeAmount);
        PaymentMethod = _transactions.Count > 1 ? PaymentMethod.Mixed : method;

        if (PaidAmount >= TotalAmount)
        {
            Status = InvoiceStatus.Paid;
            IsSettled = true;
            SettledAt = DateTime.UtcNow;
        }
        else
        {
            Status = InvoiceStatus.PartiallyPaid;
        }

        UpdatedAt = DateTime.UtcNow;
        return tx;
    }

    public void Cancel(string reason)
    {
        if (IsSettled)
            throw new DomainValidationException("Không thể hủy hóa đơn đã thanh toán hoàn tất. Phải thực hiện quy trình hoàn tiền.");

        Status = InvoiceStatus.Cancelled;
        Notes = $"[ĐÃ HỦY] {reason}. " + (Notes ?? "");
        UpdatedAt = DateTime.UtcNow;
    }
}
