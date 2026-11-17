namespace SmartRestaurant.Application.DTOs;

using SmartRestaurant.Domain.Enums;

public record KdsTicketDto(
    Guid TicketId,
    string TicketNumber,
    Guid KitchenStationId,
    string StationName,
    Guid OrderRoundId,
    string TableCode,
    int RoundNumber,
    DateTime CreatedAt,
    int ElapsedMinutes,
    string SlaColorClass,
    KitchenTicketStatus Status,
    bool IsHighPriority,
    string? Notes,
    List<KdsTicketItemDto> Items);

public record KdsTicketItemDto(
    Guid ItemId,
    Guid MenuItemId,
    string ItemName,
    int Quantity,
    string? KitchenNote,
    OrderItemStatus Status,
    bool IsRemake,
    List<string> Modifiers);

public record KdsAggregatedDishDto(
    Guid MenuItemId,
    string ItemName,
    string StationName,
    int TotalPending,
    int TotalCooking,
    int TotalReady,
    List<string> TablesSummary);

public record KdsStationSummaryDto(
    Guid StationId,
    string StationCode,
    string StationName,
    int PendingCount,
    int InPrepCount,
    int TotalActiveCount);

public record UpdateTicketStatusRequest(
    Guid TicketId,
    KitchenTicketStatus NewStatus,
    Guid StaffId);

public record UpdateItemStatusRequest(
    Guid OrderItemId,
    OrderItemStatus NewStatus,
    Guid StaffId);

public record RemakeDishRequest(
    Guid OrderItemId,
    string Reason,
    Guid ManagerId);

public record Toggle86Request(
    Guid MenuItemId,
    bool IsAvailable,
    Guid StaffId);
