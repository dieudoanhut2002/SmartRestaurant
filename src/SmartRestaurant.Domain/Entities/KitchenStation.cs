namespace SmartRestaurant.Domain.Entities;

using SmartRestaurant.Domain.Common;
using SmartRestaurant.Domain.Enums;
using SmartRestaurant.Domain.Exceptions;

public class KitchenStation : BaseEntity, IAggregateRoot
{
    public Guid BranchId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public PrepStation StationType { get; private set; } = PrepStation.HotKitchen;
    public string Description { get; private set; } = string.Empty;
    public int DisplayOrder { get; private set; }
    public string PrinterIpAddress { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;

    private readonly List<MenuItem> _menuItems = [];
    public IReadOnlyCollection<MenuItem> MenuItems => _menuItems.AsReadOnly();

    protected KitchenStation() { }

    public KitchenStation(
        Guid branchId,
        string code,
        string name,
        PrepStation stationType = PrepStation.HotKitchen,
        string description = "",
        int displayOrder = 0,
        string printerIpAddress = "")
    {
        if (branchId == Guid.Empty)
            throw new DomainValidationException("BranchId không được để trống.");
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainValidationException("Mã trạm bếp không được để trống.");
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainValidationException("Tên trạm bếp không được để trống.");

        BranchId = branchId;
        Code = code.Trim().ToUpperInvariant();
        Name = name.Trim();
        StationType = stationType;
        Description = description?.Trim() ?? string.Empty;
        DisplayOrder = displayOrder;
        PrinterIpAddress = printerIpAddress?.Trim() ?? string.Empty;
        IsActive = true;
    }

    public void UpdateDetails(string name, PrepStation stationType, string description, int displayOrder, string printerIpAddress)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainValidationException("Tên trạm bếp không được để trống.");

        Name = name.Trim();
        StationType = stationType;
        Description = description?.Trim() ?? string.Empty;
        DisplayOrder = displayOrder;
        PrinterIpAddress = printerIpAddress?.Trim() ?? string.Empty;
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
