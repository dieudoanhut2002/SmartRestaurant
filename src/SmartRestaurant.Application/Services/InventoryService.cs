namespace SmartRestaurant.Application.Services;

using Microsoft.EntityFrameworkCore;
using SmartRestaurant.Application.Common.Interfaces;
using SmartRestaurant.Application.DTOs;
using SmartRestaurant.Application.Interfaces;
using SmartRestaurant.Domain.Entities;
using SmartRestaurant.Domain.Enums;
using SmartRestaurant.Domain.Exceptions;

public class InventoryService(IAppDbContext dbContext) : IInventoryService
{
    public async Task<List<InventoryItemDto>> GetInventoryItemsAsync(Guid branchId, CancellationToken cancellationToken = default)
    {
        var items = await dbContext.InventoryItems
            .Where(i => i.BranchId == branchId)
            .OrderBy(i => i.ItemCode)
            .ToListAsync(cancellationToken);

        return items.Select(MapToDto).ToList();
    }

    public async Task<InventoryItemDto?> GetItemByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await dbContext.InventoryItems
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

        return item == null ? null : MapToDto(item);
    }

    public async Task<InventorySummaryDto> GetSummaryAsync(Guid branchId, CancellationToken cancellationToken = default)
    {
        var items = await dbContext.InventoryItems
            .Where(i => i.BranchId == branchId && i.IsActive)
            .ToListAsync(cancellationToken);

        var totalItems = items.Count;
        var lowStockItems = items.Count(i => i.AvailableStock <= i.MinimumThreshold);
        var totalPhysical = items.Sum(i => i.PhysicalStock);
        var totalReserved = items.Sum(i => i.ReservedStock);
        var totalAvailable = items.Sum(i => i.AvailableStock);
        var totalConsumed = items.Sum(i => i.ConsumedStock);
        var totalWaste = items.Sum(i => i.WasteStock);

        return new InventorySummaryDto(
            totalItems,
            lowStockItems,
            totalPhysical,
            totalReserved,
            totalAvailable,
            totalConsumed,
            totalWaste
        );
    }

    public async Task<InventoryItemDto> CreateItemAsync(CreateInventoryItemRequest request, CancellationToken cancellationToken = default)
    {
        var existing = await dbContext.InventoryItems
            .AnyAsync(i => i.BranchId == request.BranchId && i.ItemCode == request.ItemCode.Trim().ToUpperInvariant(), cancellationToken);
        if (existing)
            throw new DomainValidationException($"Mã mặt hàng kho '{request.ItemCode}' đã tồn tại.");

        var item = new InventoryItem(
            request.BranchId,
            request.ItemCode,
            request.ItemName,
            request.CategoryName,
            request.Unit,
            request.InitialPhysicalStock,
            request.MinimumThreshold,
            request.MenuItemId);

        dbContext.InventoryItems.Add(item);

        if (request.InitialPhysicalStock > 0)
        {
            var tx = new InventoryTransaction(
                request.BranchId,
                item.Id,
                item.ItemCode,
                item.ItemName,
                InventoryTransactionType.Receipt,
                request.InitialPhysicalStock,
                0,
                request.InitialPhysicalStock,
                "Khởi tạo mặt hàng ban đầu",
                "Quản Trị Viên");
            dbContext.InventoryTransactions.Add(tx);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return MapToDto(item);
    }

    public async Task<InventoryItemDto> ReceiveStockAsync(ReceiveStockRequest request, CancellationToken cancellationToken = default)
    {
        var itemId = request.InventoryItemId;
        var item = await dbContext.InventoryItems
            .FirstOrDefaultAsync(i => i.Id == itemId, cancellationToken)
            ?? throw new DomainNotFoundException($"Không tìm thấy mặt hàng kho với ID: {itemId}");

        var previousStock = item.PhysicalStock;
        item.ReceiveStock(request.Quantity);

        var tx = new InventoryTransaction(
            request.BranchId,
            item.Id,
            item.ItemCode,
            item.ItemName,
            InventoryTransactionType.Receipt,
            request.Quantity,
            previousStock,
            item.PhysicalStock,
            request.Reason,
            request.PerformedBy);

        dbContext.InventoryTransactions.Add(tx);
        await dbContext.SaveChangesAsync(cancellationToken);

        return MapToDto(item);
    }

    public async Task<InventoryItemDto> AdjustStockAsync(AdjustStockRequest request, CancellationToken cancellationToken = default)
    {
        var itemId = request.InventoryItemId;
        var item = await dbContext.InventoryItems
            .FirstOrDefaultAsync(i => i.Id == itemId, cancellationToken)
            ?? throw new DomainNotFoundException($"Không tìm thấy mặt hàng kho với ID: {itemId}");

        var previousStock = item.PhysicalStock;
        var diff = request.NewPhysicalStock - previousStock;
        item.AdjustPhysicalStock(request.NewPhysicalStock);

        var tx = new InventoryTransaction(
            request.BranchId,
            item.Id,
            item.ItemCode,
            item.ItemName,
            InventoryTransactionType.Adjustment,
            diff,
            previousStock,
            item.PhysicalStock,
            request.Reason,
            request.PerformedBy);

        dbContext.InventoryTransactions.Add(tx);
        await dbContext.SaveChangesAsync(cancellationToken);

        return MapToDto(item);
    }

    public async Task<List<InventoryTransactionDto>> GetTransactionsAsync(Guid branchId, Guid? itemId = null, CancellationToken cancellationToken = default)
    {
        var query = dbContext.InventoryTransactions
            .Where(t => t.BranchId == branchId);

        if (itemId.HasValue && itemId.Value != Guid.Empty)
        {
            query = query.Where(t => t.InventoryItemId == itemId.Value);
        }

        var list = await query
            .OrderByDescending(t => t.Timestamp)
            .Take(100)
            .ToListAsync(cancellationToken);

        return list.Select(t => new InventoryTransactionDto(
            t.Id,
            t.BranchId,
            t.InventoryItemId,
            t.ItemCode,
            t.ItemName,
            t.Type,
            FormatTxType(t.Type),
            t.Quantity,
            t.PreviousPhysicalStock,
            t.NewPhysicalStock,
            t.Reason,
            t.PerformedBy,
            t.Timestamp
        )).ToList();
    }

    private static string FormatTxType(InventoryTransactionType type) => type switch
    {
        InventoryTransactionType.Receipt => "Nhập kho",
        InventoryTransactionType.Adjustment => "Kiểm kê / Cân kho",
        InventoryTransactionType.Consumption => "Xuất phục vụ",
        InventoryTransactionType.Waste => "Xuất hủy hao hụt",
        _ => type.ToString()
    };

    private static InventoryItemDto MapToDto(InventoryItem i) => new(
        i.Id,
        i.BranchId,
        i.MenuItemId,
        i.ItemCode,
        i.ItemName,
        i.CategoryName,
        i.Unit,
        i.PhysicalStock,
        i.ReservedStock,
        i.AvailableStock,
        i.ConsumedStock,
        i.WasteStock,
        i.MinimumThreshold,
        i.AvailableStock <= i.MinimumThreshold,
        i.IsActive
    );
}
