namespace SmartRestaurant.Domain.Entities;

using SmartRestaurant.Domain.Common;
using SmartRestaurant.Domain.Enums;
using SmartRestaurant.Domain.Exceptions;

public class Order : BaseEntity, IAggregateRoot
{
    private readonly List<OrderRound> _rounds = new();

    public Guid BranchId { get; private set; }
    public Guid DiningSessionId { get; private set; }
    public string OrderCode { get; private set; } = string.Empty;
    public DateTime BusinessDate { get; private set; }
    public OrderStatus Status { get; private set; } = OrderStatus.Active;
    public decimal Subtotal { get; private set; }
    public decimal Surcharges { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public bool IsSettled { get; private set; }
    public DateTime? SettledAt { get; private set; }

    public IReadOnlyCollection<OrderRound> Rounds => _rounds.AsReadOnly();

    protected Order() { }

    public Order(Guid branchId, Guid diningSessionId, string orderCode, DateTime businessDate)
    {
        if (branchId == Guid.Empty)
            throw new DomainValidationException("BranchId không được để trống.");
        if (diningSessionId == Guid.Empty)
            throw new DomainValidationException("DiningSessionId không được để trống.");
        if (string.IsNullOrWhiteSpace(orderCode))
            throw new DomainValidationException("Mã đơn hàng không được để trống.");

        BranchId = branchId;
        DiningSessionId = diningSessionId;
        OrderCode = orderCode.Trim();
        BusinessDate = businessDate.Date;
        Status = OrderStatus.Active;
        Subtotal = 0;
        Surcharges = 0;
        DiscountAmount = 0;
        TotalAmount = 0;
        IsSettled = false;
    }

    public void AddRound(OrderRound round)
    {
        if (IsSettled)
            throw new DomainValidationException("Hóa đơn đã quyết toán là bất biến (OD-13), không thể thêm món.");
        if (round == null)
            throw new DomainValidationException("Đợt gọi món không thể null.");

        if (!_rounds.Any(r => r.Id == round.Id))
        {
            _rounds.Add(round);
        }
        RecalculateTotals();
        UpdatedAt = DateTime.UtcNow;
    }

    public void RecalculateTotals(decimal surchargePercent = 0, decimal surchargeFixed = 0, decimal discount = 0)
    {
        if (IsSettled)
            throw new DomainValidationException("Hóa đơn đã quyết toán không thể tính lại số tiền.");

        decimal sum = 0;
        foreach (var round in _rounds)
        {
            foreach (var item in round.Items)
            {
                if (item.Status != OrderItemStatus.Cancelled)
                {
                    decimal itemTotal = item.UnitPrice * item.Quantity;
                    foreach (var mod in item.Modifiers)
                    {
                        itemTotal += mod.PriceAdjustment * item.Quantity;
                    }
                    sum += itemTotal;
                }
            }
        }

        Subtotal = sum;
        decimal computedSurcharges = surchargeFixed;
        if (surchargePercent > 0)
        {
            computedSurcharges += Math.Round(Subtotal * (surchargePercent / 100m), 0);
        }
        Surcharges = computedSurcharges;
        DiscountAmount = Math.Max(0, discount);
        TotalAmount = Math.Max(0, Subtotal + Surcharges - DiscountAmount);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RequestBilling()
    {
        if (IsSettled)
            throw new DomainValidationException("Hóa đơn đã quyết toán không thể yêu cầu tạm tính.");

        Status = OrderStatus.BillingRequested;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Settle()
    {
        if (IsSettled)
            throw new DomainValidationException("Hóa đơn đã được quyết toán trước đó.");

        Status = OrderStatus.Settled;
        IsSettled = true;
        SettledAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (IsSettled)
            throw new DomainValidationException("Hóa đơn đã quyết toán không thể hủy.");

        Status = OrderStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }
}
