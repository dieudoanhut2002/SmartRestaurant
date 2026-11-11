namespace SmartRestaurant.Application.DTOs;

using SmartRestaurant.Domain.Enums;

public record DiningAreaDto(
    Guid Id,
    Guid BranchId,
    string Name,
    AreaType Type,
    int DisplayOrder,
    bool IsActive,
    int TableCount,
    List<RestaurantTableDto> Tables);

public record DiningRoomDto(
    Guid Id,
    Guid DiningAreaId,
    string Name,
    bool IsVip,
    decimal? SurchargeRate,
    decimal? SurchargeFixedAmount,
    bool IsActive);

public record RestaurantTableDto(
    Guid Id,
    Guid BranchId,
    Guid DiningAreaId,
    Guid? DiningRoomId,
    string Code,
    string Name,
    int Capacity,
    TableStatus Status,
    TableShape Shape,
    double PositionX,
    double PositionY,
    double Width,
    double Height,
    double Rotation,
    bool IsActive,
    string? CurrentSessionCode = null,
    decimal? CurrentSessionTotal = null);

public record FloorPlanLayoutDto(
    Guid CurrentAreaId,
    string CurrentAreaName,
    List<DiningAreaDto> Areas,
    List<RestaurantTableDto> Tables);

public record UpdateTablePositionRequest(
    Guid TableId,
    double PositionX,
    double PositionY,
    double Width,
    double Height,
    double Rotation);

public record CreateTableRequest(
    Guid BranchId,
    Guid DiningAreaId,
    Guid? DiningRoomId,
    string Code,
    string Name,
    int Capacity,
    TableShape Shape,
    double PositionX = 50,
    double PositionY = 50,
    double Width = 90,
    double Height = 90);

public record UpdateTableRequest(
    Guid TableId,
    string Name,
    int Capacity,
    TableShape Shape,
    Guid DiningAreaId,
    Guid? DiningRoomId);

public record CreateDiningAreaRequest(
    Guid BranchId,
    string Name,
    AreaType Type,
    int DisplayOrder = 0);

public record UpdateDiningAreaRequest(
    Guid AreaId,
    string Name,
    AreaType Type,
    int DisplayOrder);

public record CreateDiningRoomRequest(
    Guid DiningAreaId,
    string Name,
    bool IsVip,
    decimal? SurchargeRate,
    decimal? SurchargeFixedAmount);

public record UpdateDiningRoomRequest(
    Guid RoomId,
    string Name,
    bool IsVip,
    decimal? SurchargeRate,
    decimal? SurchargeFixedAmount);
