namespace SmartRestaurant.Domain.Entities;

using SmartRestaurant.Domain.Common;
using SmartRestaurant.Domain.Enums;
using SmartRestaurant.Domain.Exceptions;

public class WorkShift : BaseEntity, IAggregateRoot
{
    public Guid BranchId { get; private set; }
    public Guid StaffMemberId { get; private set; }
    public DateTime OpenedAt { get; private set; }
    public DateTime? ClosedAt { get; private set; }
    public decimal OpeningCashFloat { get; private set; }
    public decimal SystemCashTotal { get; private set; }
    public decimal? ActualCashCount { get; private set; }
    public decimal? VarianceAmount { get; private set; }
    public string Notes { get; private set; } = string.Empty;
    public ShiftStatus Status { get; private set; } = ShiftStatus.Open;

    protected WorkShift() { }

    public WorkShift(Guid branchId, Guid staffMemberId, decimal openingCashFloat, string notes = "")
    {
        if (branchId == Guid.Empty)
            throw new DomainValidationException("BranchId không được để trống.");
        if (staffMemberId == Guid.Empty)
            throw new DomainValidationException("StaffMemberId không được để trống.");
        if (openingCashFloat < 0)
            throw new DomainValidationException("Tiền quỹ đầu ca không được âm.");

        BranchId = branchId;
        StaffMemberId = staffMemberId;
        OpenedAt = DateTime.UtcNow;
        OpeningCashFloat = openingCashFloat;
        SystemCashTotal = 0;
        Notes = notes?.Trim() ?? string.Empty;
        Status = ShiftStatus.Open;
    }

    public void AddSystemCash(decimal amount)
    {
        if (Status != ShiftStatus.Open)
            throw new DomainValidationException("Không thể cập nhật doanh số cho ca đã đóng.");

        SystemCashTotal += amount;
        UpdatedAt = DateTime.UtcNow;
    }

    public void CloseShiftBlind(decimal actualCashCount, decimal systemCashTotal, string notes = "")
    {
        if (Status != ShiftStatus.Open)
            throw new DomainValidationException("Ca làm việc này đã được đóng trước đó.");
        if (actualCashCount < 0)
            throw new DomainValidationException("Tiền mặt thực đếm không được âm.");

        ClosedAt = DateTime.UtcNow;
        ActualCashCount = actualCashCount;
        SystemCashTotal = systemCashTotal;
        VarianceAmount = actualCashCount - (OpeningCashFloat + systemCashTotal);
        Notes = string.IsNullOrWhiteSpace(notes) ? Notes : notes.Trim();
        Status = ShiftStatus.Closed;
        UpdatedAt = DateTime.UtcNow;
    }
}
