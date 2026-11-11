namespace SmartRestaurant.Domain.Entities;

using SmartRestaurant.Domain.Common;
using SmartRestaurant.Domain.Enums;
using SmartRestaurant.Domain.Exceptions;

public class MenuItem : BaseEntity, IAggregateRoot
{
    public Guid BranchId { get; private set; }
    public Guid CategoryId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal BasePrice { get; private set; }
    public string ImageUrl { get; private set; } = string.Empty;
    public PrepStation PrepStation { get; private set; } = PrepStation.HotKitchen;
    public Guid? KitchenStationId { get; private set; }
    public bool DirectFOHPick { get; private set; } // REV-08 / BR-RULE-14 (Direct service bypassing kitchen)
    public bool IsAvailable { get; private set; } = true; // BR-KIT-006 (86 status / temporary out of stock)
    public bool IsCountable { get; private set; } = true; // D5 (Countable item tracking in MVP)
    public int InStockQuantity { get; private set; }
    public bool IsActive { get; private set; } = true;

    private readonly List<MenuItemVariant> _variants = [];
    public IReadOnlyCollection<MenuItemVariant> Variants => _variants.AsReadOnly();

    protected MenuItem() { }

    public MenuItem(
        Guid branchId,
        Guid categoryId,
        string code,
        string name,
        decimal basePrice,
        string description = "",
        string imageUrl = "",
        PrepStation prepStation = PrepStation.HotKitchen,
        bool directFOHPick = false,
        bool isCountable = true,
        int inStockQuantity = 100)
    {
        if (branchId == Guid.Empty)
            throw new DomainValidationException("BranchId không được để trống.");
        if (categoryId == Guid.Empty)
            throw new DomainValidationException("CategoryId không được để trống.");
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainValidationException("Mã món ăn không được để trống.");
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainValidationException("Tên món ăn không được để trống.");
        if (basePrice < 0)
            throw new DomainValidationException("Đơn giá món ăn không được âm.");

        BranchId = branchId;
        CategoryId = categoryId;
        Code = code.Trim().ToUpperInvariant();
        Name = name.Trim();
        BasePrice = basePrice;
        Description = description?.Trim() ?? string.Empty;
        ImageUrl = imageUrl?.Trim() ?? string.Empty;
        PrepStation = prepStation;
        DirectFOHPick = directFOHPick;
        IsCountable = isCountable;
        InStockQuantity = inStockQuantity >= 0 ? inStockQuantity : 0;
        IsAvailable = true;
        IsActive = true;
    }

    public void UpdateDetails(
        Guid categoryId,
        string name,
        decimal basePrice,
        string description,
        string imageUrl,
        PrepStation prepStation,
        bool directFOHPick,
        bool isCountable)
    {
        if (categoryId == Guid.Empty)
            throw new DomainValidationException("CategoryId không được để trống.");
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainValidationException("Tên món ăn không được để trống.");
        if (basePrice < 0)
            throw new DomainValidationException("Đơn giá món ăn không được âm.");

        CategoryId = categoryId;
        Name = name.Trim();
        BasePrice = basePrice;
        Description = description?.Trim() ?? string.Empty;
        ImageUrl = imageUrl?.Trim() ?? string.Empty;
        PrepStation = prepStation;
        DirectFOHPick = directFOHPick;
        IsCountable = isCountable;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AssignToStation(Guid? stationId, PrepStation prepStation)
    {
        KitchenStationId = stationId;
        PrepStation = prepStation;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ToggleAvailability()
    {
        IsAvailable = !IsAvailable;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetStockQuantity(int quantity)
    {
        InStockQuantity = quantity >= 0 ? quantity : 0;
        if (InStockQuantity == 0 && IsCountable)
        {
            IsAvailable = false; // BR-INV-004: Tự động khóa bán khi hết kho
        }
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
