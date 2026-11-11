namespace SmartRestaurant.Domain.Entities;

using SmartRestaurant.Domain.Common;
using SmartRestaurant.Domain.Enums;
using SmartRestaurant.Domain.Exceptions;

public class DiningSession : BaseEntity, IAggregateRoot
{
    public Guid BranchId { get; private set; }
    public Guid TableId { get; private set; }
    public string SessionCode { get; private set; } = string.Empty;
    public int GuestCount { get; private set; }
    public DateTime StartedAt { get; private set; }
    public DateTime? EndedAt { get; private set; }
    public DiningSessionStatus Status { get; private set; } = DiningSessionStatus.Active;
    public decimal TotalAmount { get; private set; }

    protected DiningSession() { }

    public DiningSession(Guid branchId, Guid tableId, string sessionCode, int guestCount = 2)
    {
        if (branchId == Guid.Empty)
            throw new DomainValidationException("BranchId không được để trống.");
        if (tableId == Guid.Empty)
            throw new DomainValidationException("TableId không được để trống.");
        if (string.IsNullOrWhiteSpace(sessionCode))
            throw new DomainValidationException("Mã phiên bàn không được để trống.");
        if (guestCount <= 0)
            throw new DomainValidationException("Số lượng khách phải lớn hơn 0.");

        BranchId = branchId;
        TableId = tableId;
        SessionCode = sessionCode.Trim();
        GuestCount = guestCount;
        StartedAt = DateTime.UtcNow;
        Status = DiningSessionStatus.Active;
        TotalAmount = 0;
    }

    public void AddAmount(decimal amount)
    {
        TotalAmount += amount;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete()
    {
        Status = DiningSessionStatus.Completed;
        EndedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        Status = DiningSessionStatus.Cancelled;
        EndedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
