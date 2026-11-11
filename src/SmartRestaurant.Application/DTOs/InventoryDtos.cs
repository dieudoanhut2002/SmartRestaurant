namespace SmartRestaurant.Application.DTOs;

using SmartRestaurant.Domain.Enums;

public record InventoryItemDto(
    Guid Id,
    Guid BranchId,
    Guid? MenuItemId,
    string ItemCode,
    string ItemName,
    string CategoryName,
    string Unit,
    int PhysicalStock,
    int ReservedStock,
    int AvailableStock,
    int ConsumedStock,
    int WasteStock,
    int MinimumThreshold,
    bool IsLowStock,
    bool IsActive
);

public record ReceiveStockRequest(
    Guid BranchId,
    Guid InventoryItemId,
    int Quantity,
    string Reason,
    string PerformedBy
);

public record AdjustStockRequest(
    Guid BranchId,
    Guid InventoryItemId,
    int NewPhysicalStock,
    string Reason,
    string PerformedBy
);

public record CreateInventoryItemRequest(
    Guid BranchId,
    string ItemCode,
    string ItemName,
    string CategoryName,
    string Unit,
    int InitialPhysicalStock,
    int MinimumThreshold,
    Guid? MenuItemId
);

public record InventoryTransactionDto(
    Guid Id,
    Guid BranchId,
    Guid InventoryItemId,
    string ItemCode,
    string ItemName,
    InventoryTransactionType Type,
    string TypeName,
    int Quantity,
    int PreviousPhysicalStock,
    int NewPhysicalStock,
    string Reason,
    string PerformedBy,
    DateTime Timestamp
);

public record InventorySummaryDto(
    int TotalItems,
    int LowStockItems,
    int TotalPhysicalCount,
    int TotalReservedCount,
    int TotalAvailableCount,
    int TotalConsumedCount,
    int TotalWasteCount
);
