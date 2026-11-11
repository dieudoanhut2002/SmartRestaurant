namespace SmartRestaurant.Application.Interfaces;

using SmartRestaurant.Application.DTOs;
using SmartRestaurant.Domain.Enums;

public interface IFloorPlanService
{
    Task<List<DiningAreaDto>> GetDiningAreasAsync(Guid branchId, CancellationToken ct = default);
    Task<DiningAreaDto?> GetDiningAreaByIdAsync(Guid areaId, CancellationToken ct = default);
    Task<FloorPlanLayoutDto> GetFloorPlanLayoutAsync(Guid branchId, Guid? selectedAreaId = null, CancellationToken ct = default);
    Task<DiningAreaDto> CreateDiningAreaAsync(CreateDiningAreaRequest request, CancellationToken ct = default);
    Task<DiningAreaDto> UpdateDiningAreaAsync(UpdateDiningAreaRequest request, CancellationToken ct = default);
    Task<bool> DeleteDiningAreaAsync(Guid areaId, CancellationToken ct = default);

    Task<List<DiningRoomDto>> GetRoomsByAreaAsync(Guid areaId, CancellationToken ct = default);
    Task<DiningRoomDto> CreateDiningRoomAsync(CreateDiningRoomRequest request, CancellationToken ct = default);
    Task<DiningRoomDto> UpdateDiningRoomAsync(UpdateDiningRoomRequest request, CancellationToken ct = default);
    Task<bool> DeleteDiningRoomAsync(Guid roomId, CancellationToken ct = default);

    Task<List<RestaurantTableDto>> GetTablesByAreaAsync(Guid areaId, CancellationToken ct = default);
    Task<RestaurantTableDto?> GetTableByIdAsync(Guid tableId, CancellationToken ct = default);
    Task<RestaurantTableDto> CreateTableAsync(CreateTableRequest request, CancellationToken ct = default);
    Task<RestaurantTableDto> UpdateTableAsync(UpdateTableRequest request, CancellationToken ct = default);
    Task<bool> UpdateTablePositionAsync(UpdateTablePositionRequest request, CancellationToken ct = default);
    Task<bool> UpdateTableStatusAsync(Guid tableId, TableStatus newStatus, CancellationToken ct = default);
    Task<bool> DeleteTableAsync(Guid tableId, CancellationToken ct = default);
}
