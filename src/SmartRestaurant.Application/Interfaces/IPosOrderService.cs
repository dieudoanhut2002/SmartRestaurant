namespace SmartRestaurant.Application.Interfaces;

using SmartRestaurant.Application.DTOs;

public interface IPosOrderService
{
    Task<IReadOnlyList<PosTableCardDto>> GetTablesQuickViewAsync(Guid branchId, Guid? areaId = null, CancellationToken ct = default);
    Task<PosDiningSessionDetailDto?> GetTableOrderDetailAsync(Guid tableId, CancellationToken ct = default);
    Task<PosDiningSessionDetailDto> OpenDiningSessionAsync(OpenDiningSessionRequest request, CancellationToken ct = default);
    Task<PosOrderRoundDto> FireOrderAsync(FireOrderRequest request, CancellationToken ct = default);
    Task<bool> TransferTableAsync(TransferTableRequest request, CancellationToken ct = default);
    Task<bool> MergeTablesAsync(MergeTablesRequest request, CancellationToken ct = default);
    Task<bool> RequestBillAsync(RequestBillRequest request, CancellationToken ct = default);
    Task<PosMenuCatalogDto> GetMenuCatalogAsync(Guid branchId, CancellationToken ct = default);
    Task<PosTableSyncDto> GetTableStateSyncAsync(Guid tableId, CancellationToken ct = default);
}
