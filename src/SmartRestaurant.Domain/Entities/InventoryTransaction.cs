namespace SmartRestaurant.Domain.Entities;

using SmartRestaurant.Domain.Common;
using SmartRestaurant.Domain.Enums;

public class InventoryTransaction : BaseEntity
{
    public Guid BranchId { get; private set; }
    public Guid InventoryItemId { get; private set; }
    public string ItemCode { get; private set; } = string.Empty;
    public string ItemName { get; private set; } = string.Empty;
    public InventoryTransactionType Type { get; private set; }
    public int Quantity { get; private set; }
    public int PreviousPhysicalStock { get; private set; }
    public int NewPhysicalStock { get; private set; }
    public string Reason { get; private set; } = string.Empty;
    public string PerformedBy { get; private set; } = string.Empty;
    public DateTime Timestamp { get; private set; }

    protected InventoryTransaction() { }

    public InventoryTransaction(
        Guid branchId,
        Guid inventoryItemId,
        string itemCode,
        string itemName,
        InventoryTransactionType type,
        int quantity,
        int previousPhysicalStock,
        int newPhysicalStock,
        string reason,
        string performedBy = "Thủ Kho")
    {
        BranchId = branchId;
        InventoryItemId = inventoryItemId;
        ItemCode = itemCode.Trim().ToUpperInvariant();
        ItemName = itemName.Trim();
        Type = type;
        Quantity = quantity;
        PreviousPhysicalStock = previousPhysicalStock;
        NewPhysicalStock = newPhysicalStock;
        Reason = reason?.Trim() ?? string.Empty;
        PerformedBy = performedBy?.Trim() ?? "Thủ Kho";
        Timestamp = DateTime.UtcNow;
    }
}
