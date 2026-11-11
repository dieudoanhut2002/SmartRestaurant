namespace SmartRestaurant.Domain.Entities;

using SmartRestaurant.Domain.Common;

public class ShiftClosingVarianceLog : BaseEntity
{
    public Guid ShiftId { get; private set; }
    public Guid BranchId { get; private set; }
    public decimal OpeningFloat { get; private set; }
    public decimal SystemRevenue { get; private set; }
    public decimal ExpectedAmount { get; private set; }
    public decimal ActualCountedAmount { get; private set; }
    public decimal VarianceAmount { get; private set; }
    public string CashierNotes { get; private set; } = string.Empty;
    public DateTime LoggedAt { get; private set; }

    protected ShiftClosingVarianceLog() { }

    public ShiftClosingVarianceLog(
        Guid shiftId,
        Guid branchId,
        decimal openingFloat,
        decimal systemRevenue,
        decimal actualCountedAmount,
        string cashierNotes = "")
    {
        ShiftId = shiftId;
        BranchId = branchId;
        OpeningFloat = openingFloat;
        SystemRevenue = systemRevenue;
        ExpectedAmount = openingFloat + systemRevenue;
        ActualCountedAmount = actualCountedAmount;
        VarianceAmount = actualCountedAmount - ExpectedAmount;
        CashierNotes = cashierNotes?.Trim() ?? string.Empty;
        LoggedAt = DateTime.UtcNow;
    }
}
