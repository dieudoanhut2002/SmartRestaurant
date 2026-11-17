namespace SmartRestaurant.Domain.Entities;

using SmartRestaurant.Domain.Common;
using SmartRestaurant.Domain.Exceptions;

public class OrderRound : BaseEntity
{
    private readonly List<OrderItem> _items = new();

    public Guid OrderId { get; private set; }
    public int RoundNumber { get; private set; }
    public DateTime FiredAt { get; private set; }
    public Guid FiredByStaffId { get; private set; }
    public string IdempotencyKey { get; private set; } = string.Empty;
    public string? Note { get; private set; }

    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    protected OrderRound() { }

    public OrderRound(Guid orderId, int roundNumber, Guid firedByStaffId, string idempotencyKey, string? note = null)
    {
        if (orderId == Guid.Empty)
            throw new DomainValidationException("OrderId không được để trống.");
        if (roundNumber <= 0)
            throw new DomainValidationException("Số đợt gọi món phải lớn hơn 0.");
        if (firedByStaffId == Guid.Empty)
            throw new DomainValidationException("Nhân viên báo bếp không được để trống.");
        if (string.IsNullOrWhiteSpace(idempotencyKey))
            throw new DomainValidationException("IdempotencyKey không được để trống.");

        OrderId = orderId;
        RoundNumber = roundNumber;
        FiredByStaffId = firedByStaffId;
        IdempotencyKey = idempotencyKey.Trim();
        Note = note?.Trim();
        FiredAt = DateTime.UtcNow;
    }

    public void AddItem(OrderItem item)
    {
        if (item == null)
            throw new DomainValidationException("Món gọi không thể null.");

        _items.Add(item);
        UpdatedAt = DateTime.UtcNow;
    }
}
