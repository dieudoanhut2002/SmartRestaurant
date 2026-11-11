namespace SmartRestaurant.Application.Interfaces;

using SmartRestaurant.Application.DTOs;

public interface IKitchenStationService
{
    Task<List<KitchenStationDto>> GetStationsAsync(Guid branchId, CancellationToken cancellationToken = default);
    Task<KitchenStationDto?> GetStationByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<KitchenStationDto> CreateStationAsync(CreateKitchenStationRequest request, CancellationToken cancellationToken = default);
    Task<KitchenStationDto> UpdateStationAsync(Guid id, UpdateKitchenStationRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteStationAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> AssignDishToStationAsync(AssignDishToStationRequest request, CancellationToken cancellationToken = default);
}
