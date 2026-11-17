namespace SmartRestaurant.Domain.Entities;

using SmartRestaurant.Domain.Common;
using SmartRestaurant.Domain.Enums;
using SmartRestaurant.Domain.Exceptions;

public class OrderItem : BaseEntity
{
    private readonly List<OrderItemModifier> _modifiers = new();

    public Guid OrderRoundId { get; private set; }
    public Guid MenuItemId { get; private set; }
    public Guid? MenuItemVariantId { get; private set; }
    public string ItemName { get; private set; } = string.Empty;
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }
    public string? KitchenNote { get; private set; }
    public OrderItemStatus Status { get; private set; } = OrderItemStatus.Pending;
    public bool DirectFOHPick { get; private set; }
    public Guid? KitchenStationId { get; private set; }
    public bool IsRemake { get; private set; }
    public string? RemakeReason { get; private set; }

    public IReadOnlyCollection<OrderItemModifier> Modifiers => _modifiers.AsReadOnly();

    protected OrderItem() { }

    public OrderItem(
        Guid orderRoundId,
        Guid menuItemId,
        string itemName,
        decimal unitPrice,
        int quantity,
        Guid? menuItemVariantId = null,
        string? kitchenNote = null,
        bool directFOHPick = false,
        Guid? kitchenStationId = null)
    {
        if (orderRoundId == Guid.Empty)
            throw new DomainValidationException("OrderRoundId không được để trống.");
        if (menuItemId == Guid.Empty)
            throw new DomainValidationException("MenuItemId không được để trống.");
        if (string.IsNullOrWhiteSpace(itemName))
            throw new DomainValidationException("Tên món không được để trống.");
        if (unitPrice < 0)
            throw new DomainValidationException("Đơn giá không được âm.");
        if (quantity <= 0)
            throw new DomainValidationException("Số lượng món phải lớn hơn 0.");

        OrderRoundId = orderRoundId;
        MenuItemId = menuItemId;
        ItemName = itemName.Trim();
        UnitPrice = unitPrice;
        Quantity = quantity;
        MenuItemVariantId = menuItemVariantId;
        KitchenNote = kitchenNote?.Trim();
        DirectFOHPick = directFOHPick;
        KitchenStationId = kitchenStationId;

        // Nếu là DirectFOHPick (món sảnh), lập tức sẵn sàng phục vụ
        Status = directFOHPick ? OrderItemStatus.ReadyToServe : OrderItemStatus.Pending;
    }

    public void AddModifier(OrderItemModifier modifier)
    {
        if (modifier == null)
            throw new DomainValidationException("Modifier không thể null.");

        _modifiers.Add(modifier);
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkCooking()
    {
        if (Status == OrderItemStatus.Cancelled || Status == OrderItemStatus.Served)
            throw new DomainValidationException($"Không thể chuyển sang Đang nấu khi món ở trạng thái {Status}.");

        Status = OrderItemStatus.Cooking;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkReadyToServe()
    {
        if (Status == OrderItemStatus.Cancelled || Status == OrderItemStatus.Served)
            throw new DomainValidationException($"Không thể chuyển sang Chờ bưng khi món ở trạng thái {Status}.");

        Status = OrderItemStatus.ReadyToServe;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkServed()
    {
        if (Status == OrderItemStatus.Cancelled)
            throw new DomainValidationException("Không thể chuyển sang Đã phục vụ khi món đã bị hủy.");

        Status = OrderItemStatus.Served;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsRemake(string reason)
    {
        IsRemake = true;
        RemakeReason = reason?.Trim();
        Status = OrderItemStatus.Pending;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetStatus(OrderItemStatus newStatus)
    {
        switch (newStatus)
        {
            case OrderItemStatus.Cooking:
                MarkCooking();
                break;
            case OrderItemStatus.ReadyToServe:
                MarkReadyToServe();
                break;
            case OrderItemStatus.Served:
                MarkServed();
                break;
            case OrderItemStatus.Cancelled:
                Cancel();
                break;
            default:
                Status = newStatus;
                UpdatedAt = DateTime.UtcNow;
                break;
        }
    }

    public void Cancel()
    {
        if (Status == OrderItemStatus.Served)
            throw new DomainValidationException("Món ăn đã phục vụ không thể hủy trực tiếp.");

        Status = OrderItemStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }
}
