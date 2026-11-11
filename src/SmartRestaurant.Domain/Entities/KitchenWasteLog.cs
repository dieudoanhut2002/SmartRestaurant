namespace SmartRestaurant.Domain.Entities;

using SmartRestaurant.Domain.Common;

public class KitchenWasteLog : BaseEntity
{
    public Guid BranchId { get; private set; }
    public Guid MenuItemId { get; private set; }
    public string MenuItemName { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public string Reason { get; private set; } = string.Empty; // e.g. "Cháy khét", "Khách đổi ý sau khi nấu", "Hỏng nguyên liệu"
    public decimal EstimatedLoss { get; private set; }
    public Guid? StaffMemberId { get; private set; }
    public string StaffName { get; private set; } = string.Empty;
    public DateTime LoggedAt { get; private set; }

    protected KitchenWasteLog() { }

    public KitchenWasteLog(
        Guid branchId,
        Guid menuItemId,
        string menuItemName,
        int quantity,
        string reason,
        decimal estimatedLoss,
        Guid? staffMemberId = null,
        string staffName = "")
    {
        BranchId = branchId;
        MenuItemId = menuItemId;
        MenuItemName = menuItemName.Trim();
        Quantity = quantity > 0 ? quantity : 1;
        Reason = reason?.Trim() ?? string.Empty;
        EstimatedLoss = estimatedLoss >= 0 ? estimatedLoss : 0;
        StaffMemberId = staffMemberId;
        StaffName = staffName?.Trim() ?? string.Empty;
        LoggedAt = DateTime.UtcNow;
    }
}
