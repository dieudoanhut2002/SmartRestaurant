namespace SmartRestaurant.Domain.Entities;

using SmartRestaurant.Domain.Common;
using SmartRestaurant.Domain.Enums;
using SmartRestaurant.Domain.Exceptions;

public class DiningArea : BaseEntity, IAggregateRoot
{
    public Guid BranchId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public AreaType Type { get; private set; }
    public int DisplayOrder { get; private set; }
    public bool IsActive { get; private set; } = true;

    private readonly List<DiningRoom> _rooms = [];
    public IReadOnlyCollection<DiningRoom> Rooms => _rooms.AsReadOnly();

    private readonly List<RestaurantTable> _tables = [];
    public IReadOnlyCollection<RestaurantTable> Tables => _tables.AsReadOnly();

    protected DiningArea() { }

    public DiningArea(Guid branchId, string name, AreaType type, int displayOrder = 0)
    {
        if (branchId == Guid.Empty)
            throw new DomainValidationException("BranchId không được để trống.");
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainValidationException("Tên khu vực không được để trống.");

        BranchId = branchId;
        Name = name.Trim();
        Type = type;
        DisplayOrder = displayOrder;
        IsActive = true;
    }

    public void UpdateDetails(string name, AreaType type, int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainValidationException("Tên khu vực không được để trống.");

        Name = name.Trim();
        Type = type;
        DisplayOrder = displayOrder;
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
