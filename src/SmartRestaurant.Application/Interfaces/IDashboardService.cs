namespace SmartRestaurant.Application.Interfaces;

using SmartRestaurant.Application.DTOs;

public interface IDashboardService
{
    Task<DashboardStatsDto> GetManagerDashboardStatsAsync(Guid branchId, CancellationToken ct = default);
}
