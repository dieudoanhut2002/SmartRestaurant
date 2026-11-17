namespace SmartRestaurant.Application.Interfaces;

using SmartRestaurant.Application.DTOs;
using SmartRestaurant.Domain.Enums;

public interface IKitchenService
{
    Task<IReadOnlyList<KdsStationSummaryDto>> GetStationsSummaryAsync(Guid branchId, CancellationToken ct = default);
    Task<IReadOnlyList<KdsTicketDto>> GetActiveTicketsAsync(Guid branchId, Guid? stationId = null, CancellationToken ct = default);
    Task<IReadOnlyList<KdsAggregatedDishDto>> GetAggregatedQueueAsync(Guid branchId, Guid? stationId = null, CancellationToken ct = default);
    Task<bool> UpdateTicketStatusAsync(UpdateTicketStatusRequest request, CancellationToken ct = default);
    Task<bool> UpdateOrderItemStatusAsync(UpdateItemStatusRequest request, CancellationToken ct = default);
    Task<KdsTicketDto> RemakeDishAsync(RemakeDishRequest request, CancellationToken ct = default);
    Task<bool> ToggleItem86Async(Toggle86Request request, CancellationToken ct = default);
}
