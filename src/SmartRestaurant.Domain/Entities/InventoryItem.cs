namespace SmartRestaurant.Domain.Entities;

using SmartRestaurant.Domain.Common;
using SmartRestaurant.Domain.Exceptions;

public class InventoryItem : BaseEntity, IAggregateRoot
{
    public Guid BranchId { get; private set; }
    public Guid? MenuItemId { get; private set; }
    public string ItemCode { get; private set; } = string.Empty;
    public string ItemName { get; private set; } = string.Empty;
    public string CategoryName { get; private set; } = string.Empty;
    public string Unit { get; private set; } = "Lon"; // e.g. Lon, Chai, Suất, Đĩa

    // 4 Distinct Inventory States per Task Requirements & Invariant INV-03
    public int PhysicalStock { get; private set; }
    public int ReservedStock { get; private set; }
    public int ConsumedStock { get; private set; }
    public int WasteStock { get; private set; }

    public int MinimumThreshold { get; private set; } = 10;
    public bool IsActive { get; private set; } = true;

    // Invariant INV-03 / D2: Available = Physical - Reserved
    public int AvailableStock => Math.Max(0, PhysicalStock - ReservedStock);

    protected InventoryItem() { }

    public InventoryItem(
        Guid branchId,
        string itemCode,
        string itemName,
        string categoryName,
        string unit = "Lon",
        int initialPhysicalStock = 100,
        int minimumThreshold = 10,
        Guid? menuItemId = null)
    {
        if (branchId == Guid.Empty)
            throw new DomainValidationException("BranchId không được để trống.");
        if (string.IsNullOrWhiteSpace(itemCode))
            throw new DomainValidationException("Mã mặt hàng kho không được để trống.");
        if (string.IsNullOrWhiteSpace(itemName))
            throw new DomainValidationException("Tên mặt hàng kho không được để trống.");
        if (initialPhysicalStock < 0)
            throw new DomainValidationException("Tồn kho thực tế ban đầu không được âm.");

        BranchId = branchId;
        MenuItemId = menuItemId;
        ItemCode = itemCode.Trim().ToUpperInvariant();
        ItemName = itemName.Trim();
        CategoryName = categoryName?.Trim() ?? "Đồ Uống";
        Unit = string.IsNullOrWhiteSpace(unit) ? "Lon" : unit.Trim();
        PhysicalStock = initialPhysicalStock;
        ReservedStock = 0;
        ConsumedStock = 0;
        WasteStock = 0;
        MinimumThreshold = minimumThreshold >= 0 ? minimumThreshold : 10;
        IsActive = true;
    }

    public void ReceiveStock(int quantity)
    {
        if (quantity <= 0)
            throw new DomainValidationException("Số lượng nhập kho phải lớn hơn 0.");

        PhysicalStock += quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AdjustPhysicalStock(int newPhysicalStock)
    {
        if (newPhysicalStock < 0)
            throw new DomainValidationException("Tồn kho thực tế không được âm.");
        if (newPhysicalStock < ReservedStock)
            throw new DomainValidationException($"Không thể điều chỉnh tồn kho ({newPhysicalStock}) nhỏ hơn số lượng đang tạm giữ cho khách ({ReservedStock}).");

        PhysicalStock = newPhysicalStock;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ReserveStock(int quantity)
    {
        if (quantity <= 0)
            throw new DomainValidationException("Số lượng tạm giữ phải lớn hơn 0.");
        if (quantity > AvailableStock)
            throw new DomainValidationException($"Không đủ tồn kho khả dụng để tạm giữ. Khả dụng: {AvailableStock}, yêu cầu: {quantity}.");

        ReservedStock += quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    public void CancelReservation(int quantity)
    {
        if (quantity <= 0) return;
        ReservedStock = Math.Max(0, ReservedStock - quantity);
        UpdatedAt = DateTime.UtcNow;
    }

    public void ConsumeStock(int quantity)
    {
        if (quantity <= 0) return;
        var actualToDeduct = Math.Min(quantity, PhysicalStock);
        PhysicalStock -= actualToDeduct;
        ReservedStock = Math.Max(0, ReservedStock - quantity);
        ConsumedStock += actualToDeduct;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RecordWaste(int quantity)
    {
        if (quantity <= 0) return;
        var actualToDeduct = Math.Min(quantity, PhysicalStock);
        PhysicalStock -= actualToDeduct;
        WasteStock += actualToDeduct;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateThreshold(int threshold)
    {
        MinimumThreshold = threshold >= 0 ? threshold : 0;
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
