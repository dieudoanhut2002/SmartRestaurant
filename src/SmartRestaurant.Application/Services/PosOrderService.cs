namespace SmartRestaurant.Application.Services;

using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SmartRestaurant.Application.Common.Interfaces;
using SmartRestaurant.Application.DTOs;
using SmartRestaurant.Application.Interfaces;
using SmartRestaurant.Domain.Entities;
using SmartRestaurant.Domain.Enums;
using SmartRestaurant.Domain.Exceptions;

public class PosOrderService(IAppDbContext dbContext) : IPosOrderService
{
    private readonly IAppDbContext _dbContext = dbContext;

    public async Task<IReadOnlyList<PosTableCardDto>> GetTablesQuickViewAsync(Guid branchId, Guid? areaId = null, CancellationToken ct = default)
    {
        var tablesQuery = _dbContext.RestaurantTables
            .Where(t => t.BranchId == branchId && t.IsActive);

        if (areaId.HasValue && areaId.Value != Guid.Empty)
        {
            tablesQuery = tablesQuery.Where(t => t.DiningAreaId == areaId.Value);
        }

        var tables = await tablesQuery.OrderBy(t => t.Code).ToListAsync(ct);
        var areas = await _dbContext.DiningAreas.Where(a => a.BranchId == branchId).ToListAsync(ct);
        var areaIds = areas.Select(a => a.Id).ToList();
        var rooms = await _dbContext.DiningRooms.Where(r => areaIds.Contains(r.DiningAreaId)).ToListAsync(ct);

        var activeSessions = await _dbContext.DiningSessions
            .Where(s => s.BranchId == branchId && s.Status == DiningSessionStatus.Active)
            .ToListAsync(ct);

        var activeSessionIds = activeSessions.Select(s => s.Id).ToList();

        var activeOrders = await _dbContext.Orders
            .Where(o => activeSessionIds.Contains(o.DiningSessionId) && !o.IsSettled && o.Status != OrderStatus.Cancelled)
            .ToListAsync(ct);

        var cards = new List<PosTableCardDto>();
        var now = DateTime.UtcNow;

        foreach (var t in tables)
        {
            var area = areas.FirstOrDefault(a => a.Id == t.DiningAreaId);
            var room = t.DiningRoomId.HasValue ? rooms.FirstOrDefault(r => r.Id == t.DiningRoomId.Value) : null;
            var session = activeSessions.FirstOrDefault(s => s.TableId == t.Id);
            Order? order = null;
            if (session != null)
            {
                order = activeOrders.FirstOrDefault(o => o.DiningSessionId == session.Id);
            }

            int minutesOccupied = 0;
            if (session != null)
            {
                minutesOccupied = (int)Math.Max(0, (now - session.StartedAt).TotalMinutes);
            }

            decimal totalAmount = order?.TotalAmount ?? session?.TotalAmount ?? 0;

            cards.Add(new PosTableCardDto(
                TableId: t.Id,
                Code: t.Code,
                Name: t.Name,
                Status: t.Status,
                Capacity: t.Capacity,
                AreaId: t.DiningAreaId,
                AreaName: area?.Name ?? "Khu vực",
                RoomId: t.DiningRoomId,
                RoomName: room?.Name,
                ActiveSessionId: session?.Id,
                SessionCode: session?.SessionCode,
                GuestCount: session?.GuestCount ?? 0,
                TotalAmount: totalAmount,
                SessionStartedAt: session?.StartedAt,
                MinutesOccupied: minutesOccupied
            ));
        }

        return cards;
    }

    public async Task<PosDiningSessionDetailDto?> GetTableOrderDetailAsync(Guid tableId, CancellationToken ct = default)
    {
        var table = await _dbContext.RestaurantTables
            .FirstOrDefaultAsync(t => t.Id == tableId, ct)
            ?? throw new DomainNotFoundException("Không tìm thấy bàn yêu cầu.");

        var area = await _dbContext.DiningAreas.FirstOrDefaultAsync(a => a.Id == table.DiningAreaId, ct);
        var room = table.DiningRoomId.HasValue
            ? await _dbContext.DiningRooms.FirstOrDefaultAsync(r => r.Id == table.DiningRoomId.Value, ct)
            : null;

        var session = await _dbContext.DiningSessions
            .FirstOrDefaultAsync(s => s.TableId == tableId && s.Status == DiningSessionStatus.Active, ct);

        if (session == null)
            return null;

        var order = await _dbContext.Orders
            .Include(o => o.Rounds)
            .FirstOrDefaultAsync(o => o.DiningSessionId == session.Id && !o.IsSettled && o.Status != OrderStatus.Cancelled, ct);

        List<PosOrderRoundDto> roundDtos = new();

        if (order != null)
        {
            var roundIds = order.Rounds.Select(r => r.Id).ToList();
            var items = await _dbContext.OrderItems
                .Include(i => i.Modifiers)
                .Where(i => roundIds.Contains(i.OrderRoundId))
                .ToListAsync(ct);

            var staffIds = order.Rounds.Select(r => r.FiredByStaffId).Distinct().ToList();
            var staffList = await _dbContext.StaffMembers
                .Where(s => staffIds.Contains(s.Id))
                .ToListAsync(ct);

            var stations = await _dbContext.KitchenStations.ToListAsync(ct);

            foreach (var r in order.Rounds.DistinctBy(r => r.Id).OrderBy(r => r.RoundNumber))
            {
                var staff = staffList.FirstOrDefault(s => s.Id == r.FiredByStaffId);
                var roundItems = items.Where(i => i.OrderRoundId == r.Id).ToList();

                var itemDtos = roundItems.Select(i =>
                {
                    decimal lineTotal = i.UnitPrice * i.Quantity;
                    foreach (var m in i.Modifiers)
                    {
                        lineTotal += m.PriceAdjustment * i.Quantity;
                    }

                    var station = stations.FirstOrDefault(s => s.Id == i.KitchenStationId);

                    return new PosOrderItemDto(
                        ItemId: i.Id,
                        MenuItemId: i.MenuItemId,
                        MenuItemVariantId: i.MenuItemVariantId,
                        ItemName: i.ItemName,
                        UnitPrice: i.UnitPrice,
                        Quantity: i.Quantity,
                        KitchenNote: i.KitchenNote,
                        Status: i.Status,
                        DirectFOHPick: i.DirectFOHPick,
                        KitchenStationId: i.KitchenStationId,
                        StationName: station?.Name,
                        LineTotal: lineTotal,
                        Modifiers: i.Modifiers.Select(m => new PosOrderItemModifierDto(m.ModifierOptionId, m.OptionName, m.PriceAdjustment)).ToList()
                    );
                }).ToList();

                roundDtos.Add(new PosOrderRoundDto(
                    RoundId: r.Id,
                    RoundNumber: r.RoundNumber,
                    FiredAt: r.FiredAt,
                    FiredByStaffId: r.FiredByStaffId,
                    StaffName: staff?.FullName ?? "Phục vụ",
                    IdempotencyKey: r.IdempotencyKey,
                    Note: r.Note,
                    Items: itemDtos
                ));
            }
        }

        decimal vipPercent = room?.SurchargeRate ?? 0;
        decimal vipFixed = room?.SurchargeFixedAmount ?? 0;

        return new PosDiningSessionDetailDto(
            SessionId: session.Id,
            TableId: table.Id,
            TableCode: table.Code,
            TableName: table.Name,
            AreaName: area?.Name ?? "Khu vực",
            RoomName: room?.Name,
            VipSurchargePercent: vipPercent,
            VipSurchargeFixed: vipFixed,
            GuestCount: session.GuestCount,
            StartedAt: session.StartedAt,
            Status: session.Status,
            ActiveOrderId: order?.Id,
            OrderCode: order?.OrderCode,
            Subtotal: order?.Subtotal ?? 0,
            Surcharges: order?.Surcharges ?? 0,
            TotalAmount: order?.TotalAmount ?? 0,
            IsBillingRequested: order?.Status == OrderStatus.BillingRequested || table.Status == TableStatus.BillingRequested,
            Rounds: roundDtos
        );
    }

    public async Task<PosDiningSessionDetailDto> OpenDiningSessionAsync(OpenDiningSessionRequest request, CancellationToken ct = default)
    {
        var table = await _dbContext.RestaurantTables
            .FirstOrDefaultAsync(t => t.Id == request.TableId, ct)
            ?? throw new DomainNotFoundException("Không tìm thấy bàn để mở phiên.");

        var existingSession = await _dbContext.DiningSessions
            .FirstOrDefaultAsync(s => s.TableId == request.TableId && s.Status == DiningSessionStatus.Active, ct);

        if (existingSession != null)
        {
            var detail = await GetTableOrderDetailAsync(table.Id, ct);
            return detail!;
        }

        table.Occupy();

        string sessionCode = $"SES-{DateTime.UtcNow:yyyyMMddHHmmss}-{table.Code}";
        var session = new DiningSession(table.BranchId, table.Id, sessionCode, request.GuestCount);
        _dbContext.DiningSessions.Add(session);

        // Inherit BusinessDate (Cutoff 06:00 D1)
        var businessDate = DateTime.UtcNow.TimeOfDay < TimeSpan.FromHours(6)
            ? DateTime.UtcNow.Date.AddDays(-1)
            : DateTime.UtcNow.Date;

        string orderCode = $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}-{table.Code}";
        var order = new Order(table.BranchId, session.Id, orderCode, businessDate);
        _dbContext.Orders.Add(order);

        _dbContext.OrderAuditHistories.Add(new OrderAuditHistory(
            order.Id,
            session.Id,
            "OpenSession",
            $"Mở bàn {table.Code} với {request.GuestCount} khách",
            request.StaffId
        ));

        _dbContext.OutboxMessages.Add(new OutboxMessage(
            "TableStatusChanged",
            JsonSerializer.Serialize(new { TableId = table.Id, Status = TableStatus.Occupied, TableCode = table.Code })
        ));

        await _dbContext.SaveChangesAsync(ct);

        var result = await GetTableOrderDetailAsync(table.Id, ct);
        return result!;
    }

    public async Task<PosOrderRoundDto> FireOrderAsync(FireOrderRequest request, CancellationToken ct = default)
    {
        if (request.Items == null || request.Items.Count == 0)
            throw new DomainValidationException("Danh sách món gọi không được để trống khi báo bếp.");

        if (string.IsNullOrWhiteSpace(request.IdempotencyKey))
            throw new DomainValidationException("IdempotencyKey không được để trống để đảm bảo chống gửi trùng.");

        // 1. Idempotency Check: Return existing round if already processed
        var existingRound = await _dbContext.OrderRounds
            .Include(r => r.Items)
            .ThenInclude(i => i.Modifiers)
            .FirstOrDefaultAsync(r => r.IdempotencyKey == request.IdempotencyKey.Trim(), ct);

        if (existingRound != null)
        {
            var stations = await _dbContext.KitchenStations.ToListAsync(ct);
            var staff = await _dbContext.StaffMembers.FirstOrDefaultAsync(s => s.Id == existingRound.FiredByStaffId, ct);

            return new PosOrderRoundDto(
                RoundId: existingRound.Id,
                RoundNumber: existingRound.RoundNumber,
                FiredAt: existingRound.FiredAt,
                FiredByStaffId: existingRound.FiredByStaffId,
                StaffName: staff?.FullName ?? "Phục vụ",
                IdempotencyKey: existingRound.IdempotencyKey,
                Note: existingRound.Note,
                Items: existingRound.Items.Select(i => new PosOrderItemDto(
                    ItemId: i.Id,
                    MenuItemId: i.MenuItemId,
                    MenuItemVariantId: i.MenuItemVariantId,
                    ItemName: i.ItemName,
                    UnitPrice: i.UnitPrice,
                    Quantity: i.Quantity,
                    KitchenNote: i.KitchenNote,
                    Status: i.Status,
                    DirectFOHPick: i.DirectFOHPick,
                    KitchenStationId: i.KitchenStationId,
                    StationName: stations.FirstOrDefault(s => s.Id == i.KitchenStationId)?.Name,
                    LineTotal: i.UnitPrice * i.Quantity,
                    Modifiers: i.Modifiers.Select(m => new PosOrderItemModifierDto(m.ModifierOptionId, m.OptionName, m.PriceAdjustment)).ToList()
                )).ToList()
            );
        }

        // 2. Resolve Active Session & Order
        var session = await _dbContext.DiningSessions
            .FirstOrDefaultAsync(s => s.TableId == request.TableId && s.Status == DiningSessionStatus.Active, ct)
            ?? throw new DomainNotFoundException("Bàn chưa được mở phiên dùng bữa.");

        var table = await _dbContext.RestaurantTables
            .FirstOrDefaultAsync(t => t.Id == request.TableId, ct)
            ?? throw new DomainNotFoundException("Không tìm thấy thông tin bàn.");

        var room = table.DiningRoomId.HasValue
            ? await _dbContext.DiningRooms.FirstOrDefaultAsync(r => r.Id == table.DiningRoomId.Value, ct)
            : null;

        // 3. Execute all writes inside resilient transaction via ExecutionStrategy
        return await _dbContext.ExecuteInTransactionAsync(async () =>
        {
            // Rule: If table has unpaid order (even BillingRequested), new items append directly to it (D4 / BillingRequested Option 2)
            var order = await _dbContext.Orders
                .Include(o => o.Rounds)
                .FirstOrDefaultAsync(o => o.DiningSessionId == session.Id && !o.IsSettled && o.Status != OrderStatus.Cancelled, ct);

            if (order == null)
            {
                var businessDate = DateTime.UtcNow.TimeOfDay < TimeSpan.FromHours(6)
                    ? DateTime.UtcNow.Date.AddDays(-1)
                    : DateTime.UtcNow.Date;

                string orderCode = $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}-{table.Code}";
                order = new Order(table.BranchId, session.Id, orderCode, businessDate);
                _dbContext.Orders.Add(order);
            }

            int nextRoundNumber = await _dbContext.OrderRounds.CountAsync(r => r.OrderId == order.Id, ct) + 1;
            var round = new OrderRound(order.Id, nextRoundNumber, request.StaffId, request.IdempotencyKey, request.Note);
            _dbContext.OrderRounds.Add(round);

            var menuIds = request.Items.Select(i => i.MenuItemId).Distinct().ToList();
            var menuItems = await _dbContext.MenuItems.Where(m => menuIds.Contains(m.Id)).ToListAsync(ct);
            var countableInventory = await _dbContext.InventoryItems
                .Where(inv => inv.MenuItemId != null && menuIds.Contains(inv.MenuItemId.Value))
                .ToListAsync(ct);

            var kitchenTicketsToCreate = new Dictionary<Guid, List<object>>();

            foreach (var draft in request.Items)
            {
                var menuItem = menuItems.FirstOrDefault(m => m.Id == draft.MenuItemId)
                    ?? throw new DomainValidationException($"Không tìm thấy món có ID: {draft.MenuItemId}");

                // Two-Phase Inventory Lock (D2, D5): Reserve stock only for countable items with inventory tracking
                var invItem = countableInventory.FirstOrDefault(inv => inv.MenuItemId == menuItem.Id);
                if (invItem != null)
                {
                    int prevPhysical = invItem.PhysicalStock;
                    invItem.ReserveStock(draft.Quantity);
                    _dbContext.InventoryTransactions.Add(new InventoryTransaction(
                        invItem.BranchId,
                        invItem.Id,
                        invItem.ItemCode,
                        invItem.ItemName,
                        InventoryTransactionType.Reservation,
                        draft.Quantity,
                        prevPhysical,
                        invItem.PhysicalStock,
                        $"Tạm giữ {draft.Quantity} suất khi báo bếp bàn {table.Code}",
                        "Hệ thống POS"
                    ));
                }

                bool isDirectFOH = menuItem.DirectFOHPick;
                var orderItem = new OrderItem(
                    orderRoundId: round.Id,
                    menuItemId: menuItem.Id,
                    itemName: draft.ItemName,
                    unitPrice: draft.UnitPrice,
                    quantity: draft.Quantity,
                    menuItemVariantId: draft.MenuItemVariantId,
                    kitchenNote: draft.KitchenNote,
                    directFOHPick: isDirectFOH,
                    kitchenStationId: menuItem.KitchenStationId
                );

                if (draft.Modifiers != null)
                {
                    foreach (var mod in draft.Modifiers)
                    {
                        orderItem.AddModifier(new OrderItemModifier(orderItem.Id, mod.ModifierOptionId, mod.OptionName, mod.PriceAdjustment));
                    }
                }

                round.AddItem(orderItem);
                _dbContext.OrderItems.Add(orderItem);

                // Group non-FOH items by KitchenStationId for kitchen tickets
                if (!isDirectFOH && menuItem.KitchenStationId.HasValue)
                {
                    var stationId = menuItem.KitchenStationId.Value;
                    if (!kitchenTicketsToCreate.TryGetValue(stationId, out var ticketList))
                    {
                        ticketList = new List<object>();
                        kitchenTicketsToCreate[stationId] = ticketList;
                    }

                    ticketList.Add(new
                    {
                        ItemName = orderItem.ItemName,
                        Quantity = orderItem.Quantity,
                        KitchenNote = orderItem.KitchenNote,
                        Modifiers = orderItem.Modifiers.Select(m => m.OptionName).ToList()
                    });
                }
            }

            // Create Kitchen Tickets per configured station
            foreach (var (stationId, ticketItems) in kitchenTicketsToCreate)
            {
                string ticketNumber = $"TK-{DateTime.UtcNow:yyyyMMddHHmmss}-{table.Code}-{nextRoundNumber}";
                string itemsJson = JsonSerializer.Serialize(ticketItems);
                var ticket = new KitchenTicket(stationId, round.Id, ticketNumber, itemsJson, request.Note, table.Code);
                _dbContext.KitchenTickets.Add(ticket);
            }

            // Recalculate totals on order with VIP surcharges if applicable
            decimal vipPercent = room?.SurchargeRate ?? 0;
            decimal vipFixed = room?.SurchargeFixedAmount ?? 0;
            order.AddRound(round);
            order.RecalculateTotals(surchargePercent: vipPercent, surchargeFixed: vipFixed);

            session.AddAmount(round.Items.Sum(i => i.UnitPrice * i.Quantity));

            _dbContext.OrderAuditHistories.Add(new OrderAuditHistory(
                order.Id,
                session.Id,
                "FireOrder",
                $"Báo bếp đợt {nextRoundNumber} gồm {round.Items.Count} món tại bàn {table.Code}",
                request.StaffId
            ));

            _dbContext.OutboxMessages.Add(new OutboxMessage(
                "OrderFired",
                JsonSerializer.Serialize(new
                {
                    OrderId = order.Id,
                    RoundId = round.Id,
                    TableId = table.Id,
                    TableCode = table.Code,
                    RoundNumber = nextRoundNumber,
                    ItemsCount = round.Items.Count
                })
            ));

            await _dbContext.SaveChangesAsync(ct);

            // Fetch stations & staff for return DTO
            var stationsList = await _dbContext.KitchenStations.ToListAsync(ct);
            var staffMember = await _dbContext.StaffMembers.FirstOrDefaultAsync(s => s.Id == request.StaffId, ct);

            return new PosOrderRoundDto(
                RoundId: round.Id,
                RoundNumber: round.RoundNumber,
                FiredAt: round.FiredAt,
                FiredByStaffId: round.FiredByStaffId,
                StaffName: staffMember?.FullName ?? "Phục vụ",
                IdempotencyKey: round.IdempotencyKey,
                Note: round.Note,
                Items: round.Items.Select(i => new PosOrderItemDto(
                    ItemId: i.Id,
                    MenuItemId: i.MenuItemId,
                    MenuItemVariantId: i.MenuItemVariantId,
                    ItemName: i.ItemName,
                    UnitPrice: i.UnitPrice,
                    Quantity: i.Quantity,
                    KitchenNote: i.KitchenNote,
                    Status: i.Status,
                    DirectFOHPick: i.DirectFOHPick,
                    KitchenStationId: i.KitchenStationId,
                    StationName: stationsList.FirstOrDefault(s => s.Id == i.KitchenStationId)?.Name,
                    LineTotal: i.UnitPrice * i.Quantity,
                    Modifiers: i.Modifiers.Select(m => new PosOrderItemModifierDto(m.ModifierOptionId, m.OptionName, m.PriceAdjustment)).ToList()
                )).ToList()
            );
        }, ct);
    }

    public async Task<bool> TransferTableAsync(TransferTableRequest request, CancellationToken ct = default)
    {
        var currentTable = await _dbContext.RestaurantTables.FirstOrDefaultAsync(t => t.Id == request.CurrentTableId, ct)
            ?? throw new DomainNotFoundException("Không tìm thấy bàn hiện tại.");

        var targetTable = await _dbContext.RestaurantTables.FirstOrDefaultAsync(t => t.Id == request.TargetTableId, ct)
            ?? throw new DomainNotFoundException("Không tìm thấy bàn đích.");

        if (targetTable.Status != TableStatus.Available)
            throw new DomainValidationException($"Bàn {targetTable.Code} đang có khách hoặc bận, không thể chuyển sang.");

        var session = await _dbContext.DiningSessions
            .FirstOrDefaultAsync(s => s.TableId == currentTable.Id && s.Status == DiningSessionStatus.Active, ct)
            ?? throw new DomainNotFoundException("Bàn hiện tại không có phiên dùng bữa hoạt động.");

        session.TransferToTable(targetTable.Id);
        currentTable.ResetToAvailable();
        targetTable.Occupy();

        var order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.DiningSessionId == session.Id && !o.IsSettled, ct);
        if (order != null)
        {
            _dbContext.OrderAuditHistories.Add(new OrderAuditHistory(
                order.Id,
                session.Id,
                "TransferTable",
                $"Chuyển từ bàn {currentTable.Code} sang bàn {targetTable.Code}",
                request.StaffId
            ));
        }

        _dbContext.OutboxMessages.Add(new OutboxMessage(
            "TableStatusChanged",
            JsonSerializer.Serialize(new { TableId = currentTable.Id, Status = TableStatus.Available, TableCode = currentTable.Code })
        ));

        _dbContext.OutboxMessages.Add(new OutboxMessage(
            "TableStatusChanged",
            JsonSerializer.Serialize(new { TableId = targetTable.Id, Status = TableStatus.Occupied, TableCode = targetTable.Code })
        ));

        await _dbContext.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> MergeTablesAsync(MergeTablesRequest request, CancellationToken ct = default)
    {
        var sourceTable = await _dbContext.RestaurantTables.FirstOrDefaultAsync(t => t.Id == request.SourceTableId, ct)
            ?? throw new DomainNotFoundException("Không tìm thấy bàn nguồn.");

        var targetTable = await _dbContext.RestaurantTables.FirstOrDefaultAsync(t => t.Id == request.TargetTableId, ct)
            ?? throw new DomainNotFoundException("Không tìm thấy bàn đích.");

        var sourceSession = await _dbContext.DiningSessions
            .FirstOrDefaultAsync(s => s.TableId == sourceTable.Id && s.Status == DiningSessionStatus.Active, ct)
            ?? throw new DomainNotFoundException("Bàn nguồn không có phiên dùng bữa hoạt động.");

        var targetSession = await _dbContext.DiningSessions
            .FirstOrDefaultAsync(s => s.TableId == targetTable.Id && s.Status == DiningSessionStatus.Active, ct)
            ?? throw new DomainNotFoundException("Bàn đích không có phiên dùng bữa hoạt động.");

        var sourceOrders = await _dbContext.Orders
            .Where(o => o.DiningSessionId == sourceSession.Id && !o.IsSettled)
            .ToListAsync(ct);

        var targetOrder = await _dbContext.Orders
            .FirstOrDefaultAsync(o => o.DiningSessionId == targetSession.Id && !o.IsSettled, ct);

        if (targetOrder != null && sourceOrders.Count > 0)
        {
            foreach (var so in sourceOrders)
            {
                so.Cancel();
            }
        }

        sourceSession.Complete();
        sourceTable.ResetToAvailable();

        _dbContext.OrderAuditHistories.Add(new OrderAuditHistory(
            targetOrder?.Id ?? Guid.NewGuid(),
            targetSession.Id,
            "MergeTables",
            $"Gộp bàn {sourceTable.Code} vào bàn {targetTable.Code}",
            request.StaffId
        ));

        _dbContext.OutboxMessages.Add(new OutboxMessage(
            "TableStatusChanged",
            JsonSerializer.Serialize(new { TableId = sourceTable.Id, Status = TableStatus.Available, TableCode = sourceTable.Code })
        ));

        await _dbContext.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> RequestBillAsync(RequestBillRequest request, CancellationToken ct = default)
    {
        var table = await _dbContext.RestaurantTables.FirstOrDefaultAsync(t => t.Id == request.TableId, ct)
            ?? throw new DomainNotFoundException("Không tìm thấy bàn yêu cầu.");

        var session = await _dbContext.DiningSessions
            .FirstOrDefaultAsync(s => s.TableId == table.Id && s.Status == DiningSessionStatus.Active, ct)
            ?? throw new DomainNotFoundException("Bàn không có phiên dùng bữa hoạt động.");

        var order = await _dbContext.Orders
            .FirstOrDefaultAsync(o => o.DiningSessionId == session.Id && !o.IsSettled && o.Status != OrderStatus.Cancelled, ct);

        table.RequestBilling();
        order?.RequestBilling();

        if (order != null)
        {
            _dbContext.OrderAuditHistories.Add(new OrderAuditHistory(
                order.Id,
                session.Id,
                "RequestBill",
                $"Yêu cầu tạm tính / in phiếu hóa đơn bàn {table.Code}",
                request.StaffId
            ));
        }

        _dbContext.OutboxMessages.Add(new OutboxMessage(
            "TableStatusChanged",
            JsonSerializer.Serialize(new { TableId = table.Id, Status = TableStatus.BillingRequested, TableCode = table.Code })
        ));

        await _dbContext.SaveChangesAsync(ct);
        return true;
    }

    public async Task<PosMenuCatalogDto> GetMenuCatalogAsync(Guid branchId, CancellationToken ct = default)
    {
        var categories = await _dbContext.MenuCategories
            .Where(c => c.BranchId == branchId && c.IsActive)
            .OrderBy(c => c.DisplayOrder)
            .Select(c => new PosCategoryDto(c.Id, c.Name, c.DisplayOrder))
            .ToListAsync(ct);

        var menuItems = await _dbContext.MenuItems
            .Include(m => m.Variants)
            .Where(m => m.BranchId == branchId && m.IsActive)
            .OrderBy(m => m.Name)
            .ToListAsync(ct);

        var modifierGroups = await _dbContext.ModifierGroups
            .Include(g => g.Options)
            .Where(g => g.BranchId == branchId && g.IsActive)
            .ToListAsync(ct);

        var itemIds = menuItems.Select(m => m.Id).ToList();
        var countableInventory = await _dbContext.InventoryItems
            .Where(inv => inv.MenuItemId != null && itemIds.Contains(inv.MenuItemId.Value))
            .ToListAsync(ct);

        var modGroupDtos = modifierGroups.Select(g => new PosModifierGroupDto(
            g.Id,
            g.Name,
            g.IsRequired,
            g.MinSelections,
            g.MaxSelections,
            g.Options.Where(o => o.IsActive).Select(o => new PosModifierOptionDto(o.Id, o.Name, o.AdditionalPrice)).ToList()
        )).ToList();

        var itemDtos = menuItems.Select(m =>
        {
            var inv = countableInventory.FirstOrDefault(i => i.MenuItemId == m.Id);
            bool hasInventory = inv != null;
            int availableStock = inv?.AvailableStock ?? 999;

            return new PosMenuItemDto(
                Id: m.Id,
                CategoryId: m.CategoryId,
                Name: m.Name,
                Code: m.Code,
                BasePrice: m.BasePrice,
                IsAvailable: m.IsAvailable && (!hasInventory || availableStock > 0),
                DirectFOHPick: m.DirectFOHPick,
                KitchenStationId: m.KitchenStationId,
                ImageUrl: m.ImageUrl,
                HasInventoryTracking: hasInventory,
                AvailableStock: availableStock,
                Variants: m.Variants.Select(v => new PosMenuItemVariantDto(v.Id, v.Name, v.PriceAdjustment)).ToList(),
                ModifierGroups: modGroupDtos
            );
        }).ToList();

        return new PosMenuCatalogDto(categories, itemDtos);
    }

    public async Task<PosTableSyncDto> GetTableStateSyncAsync(Guid tableId, CancellationToken ct = default)
    {
        var table = await _dbContext.RestaurantTables.FirstOrDefaultAsync(t => t.Id == tableId, ct)
            ?? throw new DomainNotFoundException("Không tìm thấy bàn để đồng bộ.");

        var detail = await GetTableOrderDetailAsync(tableId, ct);
        long serverTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        return new PosTableSyncDto(
            TableId: table.Id,
            Status: table.Status,
            SessionDetail: detail,
            ServerTimestamp: serverTimestamp
        );
    }
}
