namespace SmartRestaurant.Application.Services;

using Microsoft.EntityFrameworkCore;
using SmartRestaurant.Application.Common.Interfaces;
using SmartRestaurant.Application.DTOs;
using SmartRestaurant.Application.Interfaces;
using SmartRestaurant.Domain.Entities;
using SmartRestaurant.Domain.Enums;
using SmartRestaurant.Domain.Exceptions;
using System.Text.Json;

public class KitchenService(IAppDbContext dbContext) : IKitchenService
{
    private readonly IAppDbContext _dbContext = dbContext;

    public async Task<IReadOnlyList<KdsStationSummaryDto>> GetStationsSummaryAsync(Guid branchId, CancellationToken ct = default)
    {
        List<KitchenStation> stations = await _dbContext.KitchenStations
            .Where(s => s.BranchId == branchId && s.IsActive)
            .OrderBy(s => s.Code)
            .ToListAsync(ct);

        List<KitchenTicket> activeTickets = await _dbContext.KitchenTickets
            .Where(t => t.Status == KitchenTicketStatus.Pending || t.Status == KitchenTicketStatus.InPreparation)
            .ToListAsync(ct);

        List<KdsStationSummaryDto> list = new List<KdsStationSummaryDto>();
        foreach (KitchenStation? s in stations)
        {
            List<KitchenTicket> stationTickets = activeTickets.Where(t => t.KitchenStationId == s.Id).ToList();
            int pending = stationTickets.Count(t => t.Status == KitchenTicketStatus.Pending);
            int inPrep = stationTickets.Count(t => t.Status == KitchenTicketStatus.InPreparation);

            list.Add(new KdsStationSummaryDto(
                StationId: s.Id,
                StationCode: s.Code,
                StationName: s.Name,
                PendingCount: pending,
                InPrepCount: inPrep,
                TotalActiveCount: stationTickets.Count
            ));
        }

        return list;
    }

    public async Task<IReadOnlyList<KdsTicketDto>> GetActiveTicketsAsync(Guid branchId, Guid? stationId = null, CancellationToken ct = default)
    {
        IQueryable<KitchenTicket> query = _dbContext.KitchenTickets
            .Where(t => t.Status != KitchenTicketStatus.Completed && t.Status != KitchenTicketStatus.Cancelled);
        List<KitchenTicket> t = await query.ToListAsync();
        if (stationId.HasValue && stationId.Value != Guid.Empty)
        {
            query = query.Where(t => t.KitchenStationId == stationId.Value);
        }

        List<KitchenTicket> tickets = await query
            .OrderByDescending(t => t.IsHighPriority)
            .ThenBy(t => t.CreatedAt)
            .ToListAsync(ct);

        List<Guid> roundIds = tickets.Select(t => t.OrderRoundId).Distinct().ToList();
        List<OrderRound> rounds = await _dbContext.OrderRounds
            .Where(r => roundIds.Contains(r.Id))
            .ToListAsync(ct);

        List<Guid> orderIds = rounds.Select(r => r.OrderId).Distinct().ToList();
        List<Order> orders = await _dbContext.Orders
            .Where(o => orderIds.Contains(o.Id))
            .ToListAsync(ct);

        List<Guid> sessionIds = orders.Select(o => o.DiningSessionId).Distinct().ToList();
        List<DiningSession> sessions = await _dbContext.DiningSessions
            .Where(s => sessionIds.Contains(s.Id))
            .ToListAsync(ct);

        List<Guid> tableIds = sessions.Select(s => s.TableId).Distinct().ToList();
        List<RestaurantTable> tables = await _dbContext.RestaurantTables
            .Where(t => tableIds.Contains(t.Id))
            .ToListAsync(ct);

        List<KitchenStation> stations = await _dbContext.KitchenStations
            .Where(s => s.BranchId == branchId)
            .ToListAsync(ct);

        List<OrderItem> items = await _dbContext.OrderItems
            .Include(i => i.Modifiers)
            .Where(i => roundIds.Contains(i.OrderRoundId))
            .ToListAsync(ct);

        DateTime now = DateTime.UtcNow;
        List<KdsTicketDto> result = new List<KdsTicketDto>();

        foreach (KitchenTicket? t in tickets)
        {
            OrderRound? round = rounds.FirstOrDefault(r => r.Id == t.OrderRoundId);
            Order? order = round != null ? orders.FirstOrDefault(o => o.Id == round.OrderId) : null;
            DiningSession? session = order != null ? sessions.FirstOrDefault(s => s.Id == order.DiningSessionId) : null;
            RestaurantTable? table = session != null ? tables.FirstOrDefault(tbl => tbl.Id == session.TableId) : null;
            KitchenStation? station = stations.FirstOrDefault(s => s.Id == t.KitchenStationId);

            int elapsed = (int)Math.Max(0, (now - t.CreatedAt).TotalMinutes);
            // SLA color classification per BR-KIT-002: Green <10m, Yellow 10-20m, Red >20m
            string slaClass = elapsed >= 20 ? "sla-red" : elapsed >= 10 ? "sla-yellow" : "sla-green";

            // Only include items belonging to this ticket's station or non-FOH
            List<OrderItem> roundItems = items
                .Where(i => i.OrderRoundId == t.OrderRoundId && (i.KitchenStationId == t.KitchenStationId || !i.KitchenStationId.HasValue))
                .ToList();

            List<KdsTicketItemDto> itemDtos = roundItems.Select(i => new KdsTicketItemDto(
                ItemId: i.Id,
                MenuItemId: i.MenuItemId,
                ItemName: i.ItemName,
                Quantity: i.Quantity,
                KitchenNote: i.KitchenNote,
                Status: i.Status,
                IsRemake: i.IsRemake,
                Modifiers: i.Modifiers.Select(m => m.OptionName).ToList()
            )).ToList();

            string tableCode = !string.IsNullOrEmpty(t.TableCode)
                ? t.TableCode
                : (table?.Code ?? "BÀN ?");

            result.Add(new KdsTicketDto(
                TicketId: t.Id,
                TicketNumber: t.TicketNumber,
                KitchenStationId: t.KitchenStationId,
                StationName: station?.Name ?? "Trạm Bếp",
                OrderRoundId: t.OrderRoundId,
                TableCode: tableCode,
                RoundNumber: round?.RoundNumber ?? 1,
                CreatedAt: t.CreatedAt,
                ElapsedMinutes: elapsed,
                SlaColorClass: slaClass,
                Status: t.Status,
                IsHighPriority: t.IsHighPriority,
                Notes: t.Notes,
                Items: itemDtos
            ));
        }

        return result;
    }

    public async Task<IReadOnlyList<KdsAggregatedDishDto>> GetAggregatedQueueAsync(Guid branchId, Guid? stationId = null, CancellationToken ct = default)
    {
        IReadOnlyList<KdsTicketDto> activeTickets = await GetActiveTicketsAsync(branchId, stationId, ct);

        Dictionary<Guid, (string Name, string Station, int Pending, int Cooking, int Ready, List<string> Tables)> dishMap = new Dictionary<Guid, (string Name, string Station, int Pending, int Cooking, int Ready, List<string> Tables)>();

        foreach (KdsTicketDto ticket in activeTickets)
        {
            foreach (KdsTicketItemDto item in ticket.Items)
            {
                if (item.Status == OrderItemStatus.Served || item.Status == OrderItemStatus.Cancelled)
                    continue;

                if (!dishMap.TryGetValue(item.MenuItemId, out (string Name, string Station, int Pending, int Cooking, int Ready, List<string> Tables) entry))
                {
                    entry = (item.ItemName, ticket.StationName, 0, 0, 0, new List<string>());
                }

                int pending = entry.Pending;
                int cooking = entry.Cooking;
                int ready = entry.Ready;

                if (item.Status == OrderItemStatus.Pending) pending += item.Quantity;
                else if (item.Status == OrderItemStatus.Cooking) cooking += item.Quantity;
                else if (item.Status == OrderItemStatus.ReadyToServe) ready += item.Quantity;

                entry.Tables.Add($"{ticket.TableCode} (x{item.Quantity})");

                dishMap[item.MenuItemId] = (entry.Name, entry.Station, pending, cooking, ready, entry.Tables);
            }
        }

        List<KdsAggregatedDishDto> list = dishMap.Select(kv => new KdsAggregatedDishDto(
            MenuItemId: kv.Key,
            ItemName: kv.Value.Name,
            StationName: kv.Value.Station,
            TotalPending: kv.Value.Pending,
            TotalCooking: kv.Value.Cooking,
            TotalReady: kv.Value.Ready,
            TablesSummary: kv.Value.Tables.Distinct().ToList()
        )).OrderByDescending(d => d.TotalPending + d.TotalCooking).ToList();

        return list;
    }

    public async Task<bool> UpdateTicketStatusAsync(UpdateTicketStatusRequest request, CancellationToken ct = default)
    {
        KitchenTicket ticket = await _dbContext.KitchenTickets.FirstOrDefaultAsync(t => t.Id == request.TicketId, ct)
            ?? throw new DomainNotFoundException("Không tìm thấy phiếu bếp yêu cầu.");

        ticket.SetStatus(request.NewStatus);

        // Cascade to items in round for this station
        List<OrderItem> roundItems = await _dbContext.OrderItems
            .Where(i => i.OrderRoundId == ticket.OrderRoundId && (i.KitchenStationId == ticket.KitchenStationId || !i.KitchenStationId.HasValue))
            .ToListAsync(ct);

        if (request.NewStatus == KitchenTicketStatus.InPreparation)
        {
            foreach (OrderItem? item in roundItems.Where(i => i.Status == OrderItemStatus.Pending))
            {
                item.SetStatus(OrderItemStatus.Cooking);
            }
        }
        else if (request.NewStatus == KitchenTicketStatus.Completed)
        {
            foreach (OrderItem? item in roundItems.Where(i => i.Status == OrderItemStatus.Cooking || i.Status == OrderItemStatus.Pending))
            {
                item.SetStatus(OrderItemStatus.ReadyToServe);
            }
        }

        _dbContext.OutboxMessages.Add(new OutboxMessage(
            "KitchenTicketUpdated",
            JsonSerializer.Serialize(new
            {
                TicketId = ticket.Id,
                TicketNumber = ticket.TicketNumber,
                StationId = ticket.KitchenStationId,
                NewStatus = ticket.Status
            })
        ));

        await _dbContext.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> UpdateOrderItemStatusAsync(UpdateItemStatusRequest request, CancellationToken ct = default)
    {
        OrderItem item = await _dbContext.OrderItems.FirstOrDefaultAsync(i => i.Id == request.OrderItemId, ct)
            ?? throw new DomainNotFoundException("Không tìm thấy món ăn yêu cầu.");

        item.SetStatus(request.NewStatus);

        // Check if all items in the corresponding ticket are completed
        KitchenTicket? ticket = await _dbContext.KitchenTickets
            .FirstOrDefaultAsync(t => t.OrderRoundId == item.OrderRoundId && t.KitchenStationId == item.KitchenStationId, ct);

        if (ticket != null && ticket.Status != KitchenTicketStatus.Completed)
        {
            List<OrderItem> roundItems = await _dbContext.OrderItems
                .Where(i => i.OrderRoundId == item.OrderRoundId && i.KitchenStationId == item.KitchenStationId)
                .ToListAsync(ct);

            if (roundItems.All(i => i.Status == OrderItemStatus.ReadyToServe || i.Status == OrderItemStatus.Served || i.Status == OrderItemStatus.Cancelled))
            {
                ticket.Complete();
            }
            else if (roundItems.Any(i => i.Status == OrderItemStatus.Cooking) && ticket.Status == KitchenTicketStatus.Pending)
            {
                ticket.StartPreparation();
            }
        }

        _dbContext.OutboxMessages.Add(new OutboxMessage(
            "OrderItemStatusChanged",
            JsonSerializer.Serialize(new
            {
                ItemId = item.Id,
                ItemName = item.ItemName,
                NewStatus = item.Status,
                StationId = item.KitchenStationId
            })
        ));

        await _dbContext.SaveChangesAsync(ct);
        return true;
    }

    public async Task<KdsTicketDto> RemakeDishAsync(RemakeDishRequest request, CancellationToken ct = default)
    {
        OrderItem item = await _dbContext.OrderItems.FirstOrDefaultAsync(i => i.Id == request.OrderItemId, ct)
            ?? throw new DomainNotFoundException("Không tìm thấy món để thực hiện làm lại.");

        OrderRound round = await _dbContext.OrderRounds.FirstOrDefaultAsync(r => r.Id == item.OrderRoundId, ct)
            ?? throw new DomainNotFoundException("Không tìm thấy đợt gọi món.");

        Order order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.Id == round.OrderId, ct)
            ?? throw new DomainNotFoundException("Không tìm thấy đơn hàng.");

        DiningSession session = await _dbContext.DiningSessions.FirstOrDefaultAsync(s => s.Id == order.DiningSessionId, ct)
            ?? throw new DomainNotFoundException("Không tìm thấy phiên dùng bữa.");

        RestaurantTable? table = await _dbContext.RestaurantTables.FirstOrDefaultAsync(t => t.Id == session.TableId, ct);
        string tableCode = table?.Code ?? "BÀN ?";

        // 1. Mark item as remake in domain (Customer is NOT charged again: 0đ per PDR-015 / REV-11)
        item.MarkAsRemake(request.Reason);

        // 2. Automatically log scrap loss into KitchenWasteLog
        StaffMember? manager = await _dbContext.StaffMembers.FirstOrDefaultAsync(s => s.Id == request.ManagerId, ct);
        _dbContext.KitchenWasteLogs.Add(new KitchenWasteLog(
            branchId: order.BranchId,
            menuItemId: item.MenuItemId,
            menuItemName: item.ItemName,
            quantity: item.Quantity,
            reason: $"[LÀM LẠI MÓN LỖI BẾP] {request.Reason}",
            estimatedLoss: item.UnitPrice * item.Quantity,
            staffMemberId: request.ManagerId,
            staffName: manager?.FullName ?? "Quản Lý"
        ));

        // 3. Create high-priority Kitchen Ticket with purple styling trigger
        Guid stationId = item.KitchenStationId ?? Guid.Empty;
        if (stationId == Guid.Empty)
        {
            KitchenStation? station = await _dbContext.KitchenStations.FirstOrDefaultAsync(s => s.BranchId == order.BranchId, ct);
            stationId = station?.Id ?? Guid.NewGuid();
        }

        string ticketNumber = $"TK-REMAKE-{DateTime.UtcNow:yyyyMMddHHmmss}-{tableCode}";
        string remakeItemsJson = JsonSerializer.Serialize(new[]
        {
            new { ItemName = item.ItemName, Quantity = item.Quantity, KitchenNote = $"[LÀM LẠI] {request.Reason}" }
        });

        KitchenTicket ticket = new KitchenTicket(
            kitchenStationId: stationId,
            orderRoundId: round.Id,
            ticketNumber: ticketNumber,
            itemsJson: remakeItemsJson,
            notes: $"Làm lại món lỗi bếp: {request.Reason}",
            tableCode: tableCode,
            isHighPriority: true
        );
        _dbContext.KitchenTickets.Add(ticket);

        _dbContext.OutboxMessages.Add(new OutboxMessage(
            "OrderFired",
            JsonSerializer.Serialize(new
            {
                TicketId = ticket.Id,
                TicketNumber = ticket.TicketNumber,
                TableCode = tableCode,
                IsHighPriority = true,
                Reason = request.Reason
            })
        ));

        await _dbContext.SaveChangesAsync(ct);

        KitchenStation? stationEntity = await _dbContext.KitchenStations.FirstOrDefaultAsync(s => s.Id == stationId, ct);

        return new KdsTicketDto(
            TicketId: ticket.Id,
            TicketNumber: ticket.TicketNumber,
            KitchenStationId: stationId,
            StationName: stationEntity?.Name ?? "Trạm Bếp",
            OrderRoundId: round.Id,
            TableCode: tableCode,
            RoundNumber: round.RoundNumber,
            CreatedAt: ticket.CreatedAt,
            ElapsedMinutes: 0,
            SlaColorClass: "sla-red",
            Status: KitchenTicketStatus.Pending,
            IsHighPriority: true,
            Notes: ticket.Notes,
            Items: new List<KdsTicketItemDto>
            {
                new(item.Id, item.MenuItemId, item.ItemName, item.Quantity, ticket.Notes, OrderItemStatus.Pending, true, new())
            }
        );
    }

    public async Task<bool> ToggleItem86Async(Toggle86Request request, CancellationToken ct = default)
    {
        MenuItem menuItem = await _dbContext.MenuItems.FirstOrDefaultAsync(m => m.Id == request.MenuItemId, ct)
            ?? throw new DomainNotFoundException("Không tìm thấy món ăn yêu cầu.");

        menuItem.SetAvailability(request.IsAvailable);

        _dbContext.OutboxMessages.Add(new OutboxMessage(
            "Item86Toggled",
            JsonSerializer.Serialize(new
            {
                MenuItemId = menuItem.Id,
                ItemName = menuItem.Name,
                IsAvailable = menuItem.IsAvailable
            })
        ));

        await _dbContext.SaveChangesAsync(ct);
        return true;
    }
}
