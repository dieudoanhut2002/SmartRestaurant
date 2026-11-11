namespace SmartRestaurant.Application.DTOs;

using SmartRestaurant.Domain.Enums;

public record KitchenStationDto(
    Guid Id,
    Guid BranchId,
    string Code,
    string Name,
    PrepStation StationType,
    string StationTypeName,
    string Description,
    int DisplayOrder,
    string PrinterIpAddress,
    bool IsActive,
    int AssignedDishCount,
    List<MenuItemSummaryDto> AssignedDishes
);

public record MenuItemSummaryDto(
    Guid Id,
    string Code,
    string Name,
    string CategoryName,
    decimal BasePrice
);

public record CreateKitchenStationRequest(
    Guid BranchId,
    string Code,
    string Name,
    PrepStation StationType,
    string Description,
    int DisplayOrder,
    string PrinterIpAddress
);

public record UpdateKitchenStationRequest(
    string Name,
    PrepStation StationType,
    string Description,
    int DisplayOrder,
    string PrinterIpAddress,
    bool IsActive
);

public record AssignDishToStationRequest(
    Guid MenuItemId,
    Guid? StationId,
    PrepStation PrepStation
);
