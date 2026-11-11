namespace SmartRestaurant.Domain.Entities;

using SmartRestaurant.Domain.Common;

public class ManagerPinAuditLog : BaseEntity
{
    public Guid BranchId { get; private set; }
    public Guid ManagerStaffId { get; private set; }
    public string ManagerName { get; private set; } = string.Empty;
    public string ActionName { get; private set; } = string.Empty; // e.g. "Hủy món đã nấu", "Chiết khấu 15%", "Mở két thủ công"
    public string TargetEntity { get; private set; } = string.Empty; // e.g. "Bàn T02 / Hóa đơn #1042"
    public string Reason { get; private set; } = string.Empty;
    public DateTime Timestamp { get; private set; }

    protected ManagerPinAuditLog() { }

    public ManagerPinAuditLog(
        Guid branchId,
        Guid managerStaffId,
        string managerName,
        string actionName,
        string targetEntity,
        string reason)
    {
        BranchId = branchId;
        ManagerStaffId = managerStaffId;
        ManagerName = managerName.Trim();
        ActionName = actionName.Trim();
        TargetEntity = targetEntity.Trim();
        Reason = reason?.Trim() ?? string.Empty;
        Timestamp = DateTime.UtcNow;
    }
}
