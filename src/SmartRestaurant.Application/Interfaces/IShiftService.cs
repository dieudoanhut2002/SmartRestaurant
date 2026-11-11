namespace SmartRestaurant.Application.Interfaces;

using SmartRestaurant.Application.DTOs;

public interface IShiftService
{
    Task<List<WorkShiftDto>> GetActiveShiftsAsync(Guid branchId, CancellationToken ct = default);
    Task<List<WorkShiftDto>> GetShiftHistoryAsync(Guid branchId, int pageNumber = 1, int pageSize = 20, CancellationToken ct = default);
    Task<WorkShiftDto?> GetShiftByIdAsync(Guid shiftId, CancellationToken ct = default);
    Task<WorkShiftDto> OpenShiftAsync(OpenShiftRequest request, CancellationToken ct = default);
    Task<ShiftVarianceLogDto> CloseShiftBlindAsync(CloseShiftBlindRequest request, CancellationToken ct = default);
    Task<List<ShiftVarianceLogDto>> GetVarianceLogsAsync(Guid branchId, CancellationToken ct = default);
}
