namespace SmartRestaurant.Application.Interfaces;

using SmartRestaurant.Application.DTOs;

public interface IInventoryService
{
    Task<List<InventoryItemDto>> GetInventoryItemsAsync(Guid branchId, CancellationToken cancellationToken = default);
    Task<InventoryItemDto?> GetItemByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<InventorySummaryDto> GetSummaryAsync(Guid branchId, CancellationToken cancellationToken = default);
    Task<InventoryItemDto> CreateItemAsync(CreateInventoryItemRequest request, CancellationToken cancellationToken = default);
    Task<InventoryItemDto> ReceiveStockAsync(ReceiveStockRequest request, CancellationToken cancellationToken = default);
    Task<InventoryItemDto> AdjustStockAsync(AdjustStockRequest request, CancellationToken cancellationToken = default);
    Task<List<InventoryTransactionDto>> GetTransactionsAsync(Guid branchId, Guid? itemId = null, CancellationToken cancellationToken = default);
}
