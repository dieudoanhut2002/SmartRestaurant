namespace SmartRestaurant.Domain.Entities;

using SmartRestaurant.Domain.Common;
using SmartRestaurant.Domain.Exceptions;

public class MenuCategory : BaseEntity, IAggregateRoot
{
    public Guid BranchId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public int DisplayOrder { get; private set; }
    public Guid? ParentCategoryId { get; private set; }
    public bool IsActive { get; private set; } = true;

    private readonly List<MenuItem> _items = [];
    public IReadOnlyCollection<MenuItem> Items => _items.AsReadOnly();

    protected MenuCategory() { }

    public MenuCategory(Guid branchId, string name, string description = "", int displayOrder = 0, Guid? parentCategoryId = null)
    {
        if (branchId == Guid.Empty)
            throw new DomainValidationException("BranchId không được để trống.");
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainValidationException("Tên danh mục món không được để trống.");

        BranchId = branchId;
        Name = name.Trim();
        Description = description?.Trim() ?? string.Empty;
        DisplayOrder = displayOrder;
        ParentCategoryId = parentCategoryId;
        IsActive = true;
    }

    public void UpdateDetails(string name, string description, int displayOrder, Guid? parentCategoryId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainValidationException("Tên danh mục món không được để trống.");

        Name = name.Trim();
        Description = description?.Trim() ?? string.Empty;
        DisplayOrder = displayOrder;
        ParentCategoryId = parentCategoryId;
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
