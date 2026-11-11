namespace SmartRestaurant.Domain.Entities;

using SmartRestaurant.Domain.Common;
using SmartRestaurant.Domain.Exceptions;

public class ModifierOption : BaseEntity
{
    public Guid ModifierGroupId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public decimal AdditionalPrice { get; private set; }
    public bool IsActive { get; private set; } = true;

    protected ModifierOption() { }

    public ModifierOption(Guid modifierGroupId, string name, decimal additionalPrice = 0)
    {
        if (modifierGroupId == Guid.Empty)
            throw new DomainValidationException("ModifierGroupId không được để trống.");
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainValidationException("Tên tùy chọn không được để trống.");
        if (additionalPrice < 0)
            throw new DomainValidationException("Giá cộng thêm không được âm.");

        ModifierGroupId = modifierGroupId;
        Name = name.Trim();
        AdditionalPrice = additionalPrice;
        IsActive = true;
    }

    public void Update(string name, decimal additionalPrice)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainValidationException("Tên tùy chọn không được để trống.");
        if (additionalPrice < 0)
            throw new DomainValidationException("Giá cộng thêm không được âm.");

        Name = name.Trim();
        AdditionalPrice = additionalPrice;
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
