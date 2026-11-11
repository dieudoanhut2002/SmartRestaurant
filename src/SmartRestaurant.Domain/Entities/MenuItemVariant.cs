namespace SmartRestaurant.Domain.Entities;

using SmartRestaurant.Domain.Common;
using SmartRestaurant.Domain.Exceptions;

public class MenuItemVariant : BaseEntity
{
    public Guid MenuItemId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public decimal PriceAdjustment { get; private set; }
    public string Sku { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;

    protected MenuItemVariant() { }

    public MenuItemVariant(Guid menuItemId, string name, decimal priceAdjustment = 0, string sku = "")
    {
        if (menuItemId == Guid.Empty)
            throw new DomainValidationException("MenuItemId không được để trống.");
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainValidationException("Tên biến thể không được để trống.");

        MenuItemId = menuItemId;
        Name = name.Trim();
        PriceAdjustment = priceAdjustment;
        Sku = sku?.Trim() ?? string.Empty;
        IsActive = true;
    }

    public void Update(string name, decimal priceAdjustment, string sku)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainValidationException("Tên biến thể không được để trống.");

        Name = name.Trim();
        PriceAdjustment = priceAdjustment;
        Sku = sku?.Trim() ?? string.Empty;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }
}
