namespace SmartRestaurant.Application.Interfaces;

using SmartRestaurant.Application.DTOs;

public interface IReportService
{
    Task<DailySalesReportDto> GetDailySalesReportAsync(Guid branchId, DateTime? businessDate = null, CancellationToken ct = default);
    Task<List<TopSellingDishDto>> GetTopSellingDishesAsync(Guid branchId, int top = 10, CancellationToken ct = default);
    Task<List<KitchenWasteLogDto>> GetKitchenWasteLogsAsync(Guid branchId, CancellationToken ct = default);
    Task<KitchenWasteLogDto> LogKitchenWasteAsync(Guid branchId, Guid menuItemId, int quantity, string reason, decimal estimatedLoss, Guid? staffId, string staffName, CancellationToken ct = default);
    Task<List<ManagerPinAuditLogDto>> GetManagerPinAuditLogsAsync(Guid branchId, CancellationToken ct = default);
}
