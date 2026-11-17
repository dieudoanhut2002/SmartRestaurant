namespace SmartRestaurant.Domain.Entities;

using SmartRestaurant.Domain.Common;
using SmartRestaurant.Domain.Exceptions;

public class OrderAuditHistory : BaseEntity
{
    public Guid OrderId { get; private set; }
    public Guid DiningSessionId { get; private set; }
    public string Action { get; private set; } = string.Empty;
    public string Details { get; private set; } = string.Empty;
    public Guid PerformedByStaffId { get; private set; }
    public DateTime PerformedAt { get; private set; }

    protected OrderAuditHistory() { }

    public OrderAuditHistory(
        Guid orderId,
        Guid diningSessionId,
        string action,
        string details,
        Guid performedByStaffId)
    {
        if (orderId == Guid.Empty)
            throw new DomainValidationException("OrderId không được để trống.");
        if (diningSessionId == Guid.Empty)
            throw new DomainValidationException("DiningSessionId không được để trống.");
        if (string.IsNullOrWhiteSpace(action))
            throw new DomainValidationException("Action không được để trống.");
        if (performedByStaffId == Guid.Empty)
            throw new DomainValidationException("Nhân viên thực hiện không được để trống.");

        OrderId = orderId;
        DiningSessionId = diningSessionId;
        Action = action.Trim();
        Details = details?.Trim() ?? string.Empty;
        PerformedByStaffId = performedByStaffId;
        PerformedAt = DateTime.UtcNow;
    }
}
