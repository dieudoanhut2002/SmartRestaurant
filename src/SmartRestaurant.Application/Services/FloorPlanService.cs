namespace SmartRestaurant.Application.Services;

using Microsoft.EntityFrameworkCore;
using SmartRestaurant.Application.Common.Interfaces;
using SmartRestaurant.Application.DTOs;
using SmartRestaurant.Application.Interfaces;
using SmartRestaurant.Domain.Entities;
using SmartRestaurant.Domain.Enums;
using SmartRestaurant.Domain.Exceptions;

public class FloorPlanService(IAppDbContext dbContext) : IFloorPlanService
{
    public async Task<List<DiningAreaDto>> GetDiningAreasAsync(Guid branchId, CancellationToken ct = default)
    {
        var areas = await dbContext.DiningAreas
            .Where(a => a.BranchId == branchId && a.IsActive)
            .OrderBy(a => a.DisplayOrder)
            .ToListAsync(ct);

        var result = new List<DiningAreaDto>();
        foreach (var area in areas)
        {
            var tables = await dbContext.RestaurantTables
                .Where(t => t.DiningAreaId == area.Id && t.IsActive)
                .OrderBy(t => t.Code)
                .ToListAsync(ct);

            var tableDtos = tables.Select(t => new RestaurantTableDto(
                t.Id,
                t.BranchId,
                t.DiningAreaId,
                t.DiningRoomId,
                t.Code,
                t.Name,
                t.Capacity,
                t.Status,
                t.Shape,
                t.PositionX,
                t.PositionY,
                t.Width,
                t.Height,
                t.Rotation,
                t.IsActive)).ToList();

            result.Add(new DiningAreaDto(
                area.Id,
                area.BranchId,
                area.Name,
                area.Type,
                area.DisplayOrder,
                area.IsActive,
                tableDtos.Count,
                tableDtos));
        }

        return result;
    }

    public async Task<DiningAreaDto?> GetDiningAreaByIdAsync(Guid areaId, CancellationToken ct = default)
    {
        var area = await dbContext.DiningAreas.FirstOrDefaultAsync(a => a.Id == areaId, ct);
        if (area == null) return null;

        var tables = await dbContext.RestaurantTables
            .Where(t => t.DiningAreaId == area.Id && t.IsActive)
            .OrderBy(t => t.Code)
            .ToListAsync(ct);

        var tableDtos = tables.Select(t => new RestaurantTableDto(
            t.Id,
            t.BranchId,
            t.DiningAreaId,
            t.DiningRoomId,
            t.Code,
            t.Name,
            t.Capacity,
            t.Status,
            t.Shape,
            t.PositionX,
            t.PositionY,
            t.Width,
            t.Height,
            t.Rotation,
            t.IsActive)).ToList();

        return new DiningAreaDto(
            area.Id,
            area.BranchId,
            area.Name,
            area.Type,
            area.DisplayOrder,
            area.IsActive,
            tableDtos.Count,
            tableDtos);
    }

    public async Task<FloorPlanLayoutDto> GetFloorPlanLayoutAsync(Guid branchId, Guid? selectedAreaId = null, CancellationToken ct = default)
    {
        var allAreas = await GetDiningAreasAsync(branchId, ct);
        if (allAreas.Count == 0)
        {
            return new FloorPlanLayoutDto(Guid.Empty, "Chưa có khu vực", [], []);
        }

        var currentArea = selectedAreaId.HasValue
            ? allAreas.FirstOrDefault(a => a.Id == selectedAreaId.Value) ?? allAreas[0]
            : allAreas[0];

        // Fetch active sessions for table status enrichment
        var activeSessions = await dbContext.DiningSessions
            .Where(s => s.BranchId == branchId && s.Status == DiningSessionStatus.Active)
            .ToListAsync(ct);

        var enrichedTables = currentArea.Tables.Select(t =>
        {
            var session = activeSessions.FirstOrDefault(s => s.TableId == t.Id);
            return t with
            {
                CurrentSessionCode = session?.SessionCode,
                CurrentSessionTotal = session?.TotalAmount
            };
        }).ToList();

        return new FloorPlanLayoutDto(
            currentArea.Id,
            currentArea.Name,
            allAreas,
            enrichedTables);
    }

    public async Task<DiningAreaDto> CreateDiningAreaAsync(CreateDiningAreaRequest request, CancellationToken ct = default)
    {
        var area = new DiningArea(request.BranchId, request.Name, request.Type, request.DisplayOrder);
        dbContext.DiningAreas.Add(area);
        await dbContext.SaveChangesAsync(ct);

        return new DiningAreaDto(
            area.Id,
            area.BranchId,
            area.Name,
            area.Type,
            area.DisplayOrder,
            area.IsActive,
            0,
            []);
    }

    public async Task<DiningAreaDto> UpdateDiningAreaAsync(UpdateDiningAreaRequest request, CancellationToken ct = default)
    {
        var area = await dbContext.DiningAreas.FirstOrDefaultAsync(a => a.Id == request.AreaId, ct)
            ?? throw new DomainNotFoundException($"Không tìm thấy khu vực ăn uống với Id {request.AreaId}");

        area.UpdateDetails(request.Name, request.Type, request.DisplayOrder);
        await dbContext.SaveChangesAsync(ct);

        return (await GetDiningAreaByIdAsync(area.Id, ct))!;
    }

    public async Task<bool> DeleteDiningAreaAsync(Guid areaId, CancellationToken ct = default)
    {
        var area = await dbContext.DiningAreas.FirstOrDefaultAsync(a => a.Id == areaId, ct);
        if (area == null) return false;

        area.Deactivate();
        await dbContext.SaveChangesAsync(ct);
        return true;
    }

    public async Task<List<RestaurantTableDto>> GetTablesByAreaAsync(Guid areaId, CancellationToken ct = default)
    {
        var tables = await dbContext.RestaurantTables
            .Where(t => t.DiningAreaId == areaId && t.IsActive)
            .OrderBy(t => t.Code)
            .ToListAsync(ct);

        return tables.Select(t => new RestaurantTableDto(
            t.Id,
            t.BranchId,
            t.DiningAreaId,
            t.DiningRoomId,
            t.Code,
            t.Name,
            t.Capacity,
            t.Status,
            t.Shape,
            t.PositionX,
            t.PositionY,
            t.Width,
            t.Height,
            t.Rotation,
            t.IsActive)).ToList();
    }

    public async Task<RestaurantTableDto?> GetTableByIdAsync(Guid tableId, CancellationToken ct = default)
    {
        var table = await dbContext.RestaurantTables.FirstOrDefaultAsync(t => t.Id == tableId, ct);
        if (table == null) return null;

        return new RestaurantTableDto(
            table.Id,
            table.BranchId,
            table.DiningAreaId,
            table.DiningRoomId,
            table.Code,
            table.Name,
            table.Capacity,
            table.Status,
            table.Shape,
            table.PositionX,
            table.PositionY,
            table.Width,
            table.Height,
            table.Rotation,
            table.IsActive);
    }

    public async Task<RestaurantTableDto> CreateTableAsync(CreateTableRequest request, CancellationToken ct = default)
    {
        var exists = await dbContext.RestaurantTables.AnyAsync(
            t => t.BranchId == request.BranchId && t.Code == request.Code.Trim().ToUpperInvariant() && t.IsActive, ct);

        if (exists)
            throw new DomainValidationException($"Mã bàn '{request.Code}' đã tồn tại trong chi nhánh.");

        var table = new RestaurantTable(
            request.BranchId,
            request.DiningAreaId,
            request.Code,
            request.Name,
            request.Capacity,
            request.DiningRoomId,
            request.Shape,
            request.PositionX,
            request.PositionY,
            request.Width,
            request.Height);

        dbContext.RestaurantTables.Add(table);
        await dbContext.SaveChangesAsync(ct);

        return new RestaurantTableDto(
            table.Id,
            table.BranchId,
            table.DiningAreaId,
            table.DiningRoomId,
            table.Code,
            table.Name,
            table.Capacity,
            table.Status,
            table.Shape,
            table.PositionX,
            table.PositionY,
            table.Width,
            table.Height,
            table.Rotation,
            table.IsActive);
    }

    public async Task<RestaurantTableDto> UpdateTableAsync(UpdateTableRequest request, CancellationToken ct = default)
    {
        var table = await dbContext.RestaurantTables.FirstOrDefaultAsync(t => t.Id == request.TableId, ct)
            ?? throw new DomainNotFoundException($"Không tìm thấy bàn với Id {request.TableId}");

        table.UpdateInfo(request.Name, request.Capacity, request.Shape);
        await dbContext.SaveChangesAsync(ct);

        return new RestaurantTableDto(
            table.Id,
            table.BranchId,
            table.DiningAreaId,
            table.DiningRoomId,
            table.Code,
            table.Name,
            table.Capacity,
            table.Status,
            table.Shape,
            table.PositionX,
            table.PositionY,
            table.Width,
            table.Height,
            table.Rotation,
            table.IsActive);
    }

    public async Task<bool> UpdateTablePositionAsync(UpdateTablePositionRequest request, CancellationToken ct = default)
    {
        var table = await dbContext.RestaurantTables.FirstOrDefaultAsync(t => t.Id == request.TableId, ct);
        if (table == null) return false;

        table.UpdateLayout(request.PositionX, request.PositionY, request.Width, request.Height, request.Rotation);
        await dbContext.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> UpdateTableStatusAsync(Guid tableId, TableStatus newStatus, CancellationToken ct = default)
    {
        var table = await dbContext.RestaurantTables.FirstOrDefaultAsync(t => t.Id == tableId, ct);
        if (table == null) return false;

        switch (newStatus)
        {
            case TableStatus.Occupied:
                table.Occupy();
                break;
            case TableStatus.BillingRequested:
                table.RequestBilling();
                break;
            case TableStatus.CleaningNeeded:
                table.ReleaseToCleaning();
                break;
            case TableStatus.Available:
                if (table.Status == TableStatus.CleaningNeeded)
                    table.CompleteCleaning();
                else if (table.Status == TableStatus.Reserved)
                    table.CancelReservation();
                break;
            case TableStatus.Reserved:
                table.Reserve();
                break;
        }

        await dbContext.SaveChangesAsync(ct);
        return true;
    }

    public async Task<List<DiningRoomDto>> GetRoomsByAreaAsync(Guid areaId, CancellationToken ct = default)
    {
        var rooms = await dbContext.DiningRooms
            .Where(r => r.DiningAreaId == areaId)
            .OrderBy(r => r.Name)
            .ToListAsync(ct);

        return rooms.Select(r => new DiningRoomDto(
            r.Id,
            r.DiningAreaId,
            r.Name,
            r.IsVip,
            r.SurchargeRate,
            r.SurchargeFixedAmount,
            r.IsActive
        )).ToList();
    }

    public async Task<DiningRoomDto> CreateDiningRoomAsync(CreateDiningRoomRequest request, CancellationToken ct = default)
    {
        var area = await dbContext.DiningAreas.FirstOrDefaultAsync(a => a.Id == request.DiningAreaId, ct)
            ?? throw new DomainNotFoundException($"Không tìm thấy khu vực với ID: {request.DiningAreaId}");

        var room = new DiningRoom(
            request.DiningAreaId,
            request.Name,
            request.IsVip,
            request.SurchargeRate,
            request.SurchargeFixedAmount);

        dbContext.DiningRooms.Add(room);
        await dbContext.SaveChangesAsync(ct);

        return new DiningRoomDto(
            room.Id,
            room.DiningAreaId,
            room.Name,
            room.IsVip,
            room.SurchargeRate,
            room.SurchargeFixedAmount,
            room.IsActive);
    }

    public async Task<DiningRoomDto> UpdateDiningRoomAsync(UpdateDiningRoomRequest request, CancellationToken ct = default)
    {
        var room = await dbContext.DiningRooms.FirstOrDefaultAsync(r => r.Id == request.RoomId, ct)
            ?? throw new DomainNotFoundException($"Không tìm thấy phòng với ID: {request.RoomId}");

        room.UpdateDetails(request.Name, request.IsVip, request.SurchargeRate, request.SurchargeFixedAmount);
        await dbContext.SaveChangesAsync(ct);

        return new DiningRoomDto(
            room.Id,
            room.DiningAreaId,
            room.Name,
            room.IsVip,
            room.SurchargeRate,
            room.SurchargeFixedAmount,
            room.IsActive);
    }

    public async Task<bool> DeleteDiningRoomAsync(Guid roomId, CancellationToken ct = default)
    {
        var room = await dbContext.DiningRooms.FirstOrDefaultAsync(r => r.Id == roomId, ct);
        if (room == null) return false;

        room.SoftDelete();
        await dbContext.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteTableAsync(Guid tableId, CancellationToken ct = default)
    {
        var table = await dbContext.RestaurantTables.FirstOrDefaultAsync(t => t.Id == tableId, ct);
        if (table == null) return false;

        table.Deactivate();
        await dbContext.SaveChangesAsync(ct);
        return true;
    }
}
