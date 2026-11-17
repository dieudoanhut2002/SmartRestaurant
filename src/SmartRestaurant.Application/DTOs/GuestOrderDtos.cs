namespace SmartRestaurant.Application.DTOs;

using SmartRestaurant.Domain.Enums;

public record GuestTableSessionDto(
    Guid TableId,
    string TableCode,
    string TableName,
    string AreaName,
    string? RoomName,
    TableStatus Status,
    bool CanOrder,
    Guid? SessionId,
    string? SessionCode,
    List<GuestOrderedItemDto> OrderedItems,
    decimal TotalBillAmount,
    string? DynamicVietQrText,
    string? VietQrQuickLink
);

public record GuestOrderedItemDto(
    Guid ItemId,
    string ItemName,
    int Quantity,
    decimal UnitPrice,
    decimal LineTotal,
    OrderItemStatus Status,
    string StatusText,
    string StatusBadgeClass,
    string? KitchenNote,
    List<string> Modifiers
);

public class GuestOrderRequestDto
{
    public Guid TableId { get; set; }
    public string Token { get; set; } = string.Empty;
    public string? IdempotencyKey { get; set; }
    public string? CustomerNote { get; set; }
    public List<GuestOrderItemRequestDto> Items { get; set; } = [];
}

public class GuestOrderItemRequestDto
{
    public Guid MenuItemId { get; set; }
    public Guid? MenuItemVariantId { get; set; }
    public int Quantity { get; set; } = 1;
    public string? KitchenNote { get; set; }
    public List<Guid> ModifierOptionIds { get; set; } = [];
}

public record GuestOrderResultDto(
    bool Success,
    string Message,
    int RoundNumber,
    int TotalItemsOrdered,
    decimal RoundTotalAmount,
    decimal NewSessionTotal
);

public class GuestRequestBillDto
{
    public Guid TableId { get; set; }
    public string Token { get; set; } = string.Empty;
}

public record TableQrInfoDto(
    Guid TableId,
    string TableCode,
    string TableName,
    string AreaName,
    string Token,
    string QrUrl
);
