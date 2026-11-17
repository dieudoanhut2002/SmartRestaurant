namespace SmartRestaurant.Application.DTOs;

using SmartRestaurant.Domain.Enums;

public record PosTableCardDto(
    Guid TableId,
    string Code,
    string Name,
    TableStatus Status,
    int Capacity,
    Guid AreaId,
    string AreaName,
    Guid? RoomId,
    string? RoomName,
    Guid? ActiveSessionId,
    string? SessionCode,
    int GuestCount,
    decimal TotalAmount,
    DateTime? SessionStartedAt,
    int MinutesOccupied);

public record PosDiningSessionDetailDto(
    Guid SessionId,
    Guid TableId,
    string TableCode,
    string TableName,
    string AreaName,
    string? RoomName,
    decimal VipSurchargePercent,
    decimal VipSurchargeFixed,
    int GuestCount,
    DateTime StartedAt,
    DiningSessionStatus Status,
    Guid? ActiveOrderId,
    string? OrderCode,
    decimal Subtotal,
    decimal Surcharges,
    decimal TotalAmount,
    bool IsBillingRequested,
    List<PosOrderRoundDto> Rounds);

public record PosOrderRoundDto(
    Guid RoundId,
    int RoundNumber,
    DateTime FiredAt,
    Guid FiredByStaffId,
    string? StaffName,
    string IdempotencyKey,
    string? Note,
    List<PosOrderItemDto> Items);

public record PosOrderItemDto(
    Guid ItemId,
    Guid MenuItemId,
    Guid? MenuItemVariantId,
    string ItemName,
    decimal UnitPrice,
    int Quantity,
    string? KitchenNote,
    OrderItemStatus Status,
    bool DirectFOHPick,
    Guid? KitchenStationId,
    string? StationName,
    decimal LineTotal,
    List<PosOrderItemModifierDto> Modifiers);

public record PosOrderItemModifierDto(
    Guid ModifierOptionId,
    string OptionName,
    decimal PriceAdjustment);

public record PosMenuCatalogDto(
    List<PosCategoryDto> Categories,
    List<PosMenuItemDto> Items);

public record PosCategoryDto(
    Guid Id,
    string Name,
    int DisplayOrder);

public record PosMenuItemDto(
    Guid Id,
    Guid CategoryId,
    string Name,
    string Code,
    decimal BasePrice,
    bool IsAvailable,
    bool DirectFOHPick,
    Guid? KitchenStationId,
    string? ImageUrl,
    bool HasInventoryTracking,
    int AvailableStock,
    List<PosMenuItemVariantDto> Variants,
    List<PosModifierGroupDto> ModifierGroups);

public record PosMenuItemVariantDto(
    Guid Id,
    string VariantName,
    decimal PriceDifference);

public record PosModifierGroupDto(
    Guid Id,
    string GroupName,
    bool IsRequired,
    int MinSelections,
    int MaxSelections,
    List<PosModifierOptionDto> Options);

public record PosModifierOptionDto(
    Guid Id,
    string OptionName,
    decimal PriceAdjustment);

public record OpenDiningSessionRequest(
    Guid TableId,
    int GuestCount,
    Guid StaffId);

public record FireOrderRequest(
    Guid TableId,
    Guid? OrderId,
    string IdempotencyKey,
    Guid StaffId,
    string? Note,
    List<FireOrderItemDraftDto> Items);

public record FireOrderItemDraftDto(
    Guid MenuItemId,
    Guid? MenuItemVariantId,
    string ItemName,
    decimal UnitPrice,
    int Quantity,
    string? KitchenNote,
    List<FireOrderModifierDraftDto>? Modifiers);

public record FireOrderModifierDraftDto(
    Guid ModifierOptionId,
    string OptionName,
    decimal PriceAdjustment);

public record TransferTableRequest(
    Guid CurrentTableId,
    Guid TargetTableId,
    Guid StaffId);

public record MergeTablesRequest(
    Guid SourceTableId,
    Guid TargetTableId,
    Guid StaffId);

public record RequestBillRequest(
    Guid TableId,
    Guid StaffId);

public record PosTableSyncDto(
    Guid TableId,
    TableStatus Status,
    PosDiningSessionDetailDto? SessionDetail,
    long ServerTimestamp);
