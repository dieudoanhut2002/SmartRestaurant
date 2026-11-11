namespace SmartRestaurant.Domain.Exceptions;

using SmartRestaurant.Domain.Enums;

public class InvalidTableStateTransitionException(string tableCode, TableStatus currentStatus, TableStatus targetStatus)
    : DomainException($"Bàn '{tableCode}' không thể chuyển từ trạng thái '{currentStatus}' sang '{targetStatus}'.")
{
    public string TableCode { get; } = tableCode;
    public TableStatus CurrentStatus { get; } = currentStatus;
    public TableStatus TargetStatus { get; } = targetStatus;
}
