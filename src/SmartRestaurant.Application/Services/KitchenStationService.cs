namespace SmartRestaurant.Application.Services;

using Microsoft.EntityFrameworkCore;
using SmartRestaurant.Application.Common.Interfaces;
using SmartRestaurant.Application.DTOs;
using SmartRestaurant.Application.Interfaces;
using SmartRestaurant.Domain.Entities;
using SmartRestaurant.Domain.Enums;
using SmartRestaurant.Domain.Exceptions;

public class KitchenStationService(IAppDbContext dbContext) : IKitchenStationService
{
    public async Task<List<KitchenStationDto>> GetStationsAsync(Guid branchId, CancellationToken cancellationToken = default)
    {
        var stations = await dbContext.KitchenStations
            .Where(s => s.BranchId == branchId)
            .OrderBy(s => s.DisplayOrder)
            .ThenBy(s => s.Name)
            .Include(s => s.MenuItems)
            .ToListAsync(cancellationToken);

        var categories = await dbContext.MenuCategories
            .Where(c => c.BranchId == branchId)
            .ToDictionaryAsync(c => c.Id, c => c.Name, cancellationToken);

        return stations.Select(s => MapToDto(s, categories)).ToList();
    }

    public async Task<KitchenStationDto?> GetStationByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var station = await dbContext.KitchenStations
            .Include(s => s.MenuItems)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        if (station == null) return null;

        var categories = await dbContext.MenuCategories
            .Where(c => c.BranchId == station.BranchId)
            .ToDictionaryAsync(c => c.Id, c => c.Name, cancellationToken);

        return MapToDto(station, categories);
    }

    public async Task<KitchenStationDto> CreateStationAsync(CreateKitchenStationRequest request, CancellationToken cancellationToken = default)
    {
        var existing = await dbContext.KitchenStations
            .AnyAsync(s => s.BranchId == request.BranchId && s.Code == request.Code.Trim().ToUpperInvariant(), cancellationToken);
        if (existing)
            throw new DomainValidationException($"Mã trạm bếp '{request.Code}' đã tồn tại trong chi nhánh.");

        var station = new KitchenStation(
            request.BranchId,
            request.Code,
            request.Name,
            request.StationType,
            request.Description,
            request.DisplayOrder,
            request.PrinterIpAddress);

        dbContext.KitchenStations.Add(station);
        await dbContext.SaveChangesAsync(cancellationToken);

        return MapToDto(station, new Dictionary<Guid, string>());
    }

    public async Task<KitchenStationDto> UpdateStationAsync(Guid id, UpdateKitchenStationRequest request, CancellationToken cancellationToken = default)
    {
        var station = await dbContext.KitchenStations
            .Include(s => s.MenuItems)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken)
            ?? throw new DomainNotFoundException($"Không tìm thấy trạm chế biến với ID: {id}");

        station.UpdateDetails(
            request.Name,
            request.StationType,
            request.Description,
            request.DisplayOrder,
            request.PrinterIpAddress);

        if (request.IsActive)
            station.Activate();
        else
            station.Deactivate();

        await dbContext.SaveChangesAsync(cancellationToken);

        var categories = await dbContext.MenuCategories
            .Where(c => c.BranchId == station.BranchId)
            .ToDictionaryAsync(c => c.Id, c => c.Name, cancellationToken);

        return MapToDto(station, categories);
    }

    public async Task<bool> DeleteStationAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var station = await dbContext.KitchenStations
            .Include(s => s.MenuItems)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        if (station == null) return false;

        // Unlink any dishes from this station
        foreach (var dish in station.MenuItems)
        {
            dish.AssignToStation(null, station.StationType);
        }

        station.SoftDelete();
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> AssignDishToStationAsync(AssignDishToStationRequest request, CancellationToken cancellationToken = default)
    {
        var dishId = request.MenuItemId;
        var item = await dbContext.MenuItems
            .FirstOrDefaultAsync(m => m.Id == dishId, cancellationToken)
            ?? throw new DomainNotFoundException($"Không tìm thấy món ăn với ID: {dishId}");

        item.AssignToStation(request.StationId, request.PrepStation);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static KitchenStationDto MapToDto(KitchenStation s, Dictionary<Guid, string> categoryNames)
    {
        var typeName = s.StationType switch
        {
            PrepStation.HotKitchen => "Bếp Nóng",
            PrepStation.ColdKitchen => "Bếp Nguội & Salad",
            PrepStation.Bar => "Quầy Bar & Pha Chế",
            PrepStation.Bakery => "Bếp Bánh & Tráng Miệng",
            _ => s.StationType.ToString()
        };

        var dishes = s.MenuItems.Select(m => new MenuItemSummaryDto(
            m.Id,
            m.Code,
            m.Name,
            categoryNames.TryGetValue(m.CategoryId, out var cat) ? cat : "Chưa phân loại",
            m.BasePrice
        )).ToList();

        return new KitchenStationDto(
            s.Id,
            s.BranchId,
            s.Code,
            s.Name,
            s.StationType,
            typeName,
            s.Description,
            s.DisplayOrder,
            s.PrinterIpAddress,
            s.IsActive,
            dishes.Count,
            dishes
        );
    }
}
