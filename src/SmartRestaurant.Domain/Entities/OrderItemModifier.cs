namespace SmartRestaurant.Domain.Entities;

using SmartRestaurant.Domain.Common;
using SmartRestaurant.Domain.Exceptions;

public class OrderItemModifier : BaseEntity
{
    public Guid OrderItemId { get; private set; }
    public Guid ModifierOptionId { get; private set; }
    public string OptionName { get; private set; } = string.Empty;
    public decimal PriceAdjustment { get; private set; }

    protected OrderItemModifier() { }

    public OrderItemModifier(Guid orderItemId, Guid modifierOptionId, string optionName, decimal priceAdjustment = 0)
    {
        if (orderItemId == Guid.Empty)
            throw new DomainValidationException("OrderItemId không được để trống.");
        if (modifierOptionId == Guid.Empty)
            throw new DomainValidationException("ModifierOptionId không được để trống.");
        if (string.IsNullOrWhiteSpace(optionName))
            throw new DomainValidationException("Tên tùy chọn không được để trống.");

        OrderItemId = orderItemId;
        ModifierOptionId = modifierOptionId;
        OptionName = optionName.Trim();
        PriceAdjustment = priceAdjustment;
    }
}
