namespace SmartRestaurant.Application.Services;

using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SmartRestaurant.Application.Common.Interfaces;
using SmartRestaurant.Application.DTOs;
using SmartRestaurant.Application.Interfaces;
using SmartRestaurant.Domain.Entities;
using SmartRestaurant.Domain.Enums;
using SmartRestaurant.Domain.Exceptions;

public class GuestOrderService(
    IAppDbContext dbContext,
    ITableQrTokenService qrTokenService,
    IVietQrService vietQrService) : IGuestOrderService
{
    private readonly IAppDbContext _dbContext = dbContext;
    private readonly ITableQrTokenService _qrTokenService = qrTokenService;
    private readonly IVietQrService _vietQrService = vietQrService;

    public async Task<GuestTableSessionDto> GetTableSessionAsync(Guid tableId, string? token, CancellationToken ct = default)
    {
        if (!_qrTokenService.ValidateToken(tableId, token))
        {
            throw new DomainValidationException("Mã QR bảo mật của bàn không hợp lệ hoặc đã bị thay đổi.");
        }

        var table = await _dbContext.RestaurantTables
            .FirstOrDefaultAsync(t => t.Id == tableId, ct)
            ?? throw new DomainNotFoundException("Không tìm thấy bàn yêu cầu.");

        var area = await _dbContext.DiningAreas.FirstOrDefaultAsync(a => a.Id == table.DiningAreaId, ct);
        var room = table.DiningRoomId.HasValue 
            ? await _dbContext.DiningRooms.FirstOrDefaultAsync(r => r.Id == table.DiningRoomId.Value, ct) 
            : null;

        var activeSession = await _dbContext.DiningSessions
            .FirstOrDefaultAsync(s => s.TableId == tableId && s.Status == DiningSessionStatus.Active, ct);

        Order? activeOrder = null;
        List<GuestOrderedItemDto> orderedItems = [];
        decimal totalAmount = 0;

        if (activeSession != null)
        {
            activeOrder = await _dbContext.Orders
                .Include(o => o.Rounds)
                .ThenInclude(r => r.Items)
                .ThenInclude(i => i.Modifiers)
                .FirstOrDefaultAsync(o => o.DiningSessionId == activeSession.Id && !o.IsSettled && o.Status != OrderStatus.Cancelled, ct);

            if (activeOrder != null)
            {
                totalAmount = activeOrder.TotalAmount;

                foreach (var round in activeOrder.Rounds.OrderBy(r => r.RoundNumber))
                {
                    foreach (var item in round.Items)
                    {
                        var (statusText, badgeClass) = item.Status switch
                        {
                            OrderItemStatus.Pending => ("Chờ bếp nhận", "badge bg-warning text-dark"),
                            OrderItemStatus.Cooking => ("Bếp đang nấu", "badge bg-primary"),
                            OrderItemStatus.ReadyToServe => ("Món đã xong", "badge bg-info text-dark"),
                            OrderItemStatus.Served => ("Đã phục vụ", "badge bg-success"),
                            OrderItemStatus.Cancelled => ("Đã hủy", "badge bg-secondary"),
                            _ => ("Đang xử lý", "badge bg-light text-dark")
                        };

                        orderedItems.Add(new GuestOrderedItemDto(
                            ItemId: item.Id,
                            ItemName: item.ItemName,
                            Quantity: item.Quantity,
                            UnitPrice: item.UnitPrice,
                            LineTotal: item.UnitPrice * item.Quantity,
                            Status: item.Status,
                            StatusText: statusText,
                            StatusBadgeClass: badgeClass,
                            KitchenNote: item.KitchenNote,
                            Modifiers: item.Modifiers.Select(m => m.OptionName).ToList()
                        ));
                    }
                }
            }
            else
            {
                totalAmount = activeSession.TotalAmount;
            }
        }

        string? dynamicVietQrText = null;
        string? vietQrQuickLink = null;

        if (totalAmount > 0)
        {
            var invoiceRef = activeOrder?.OrderCode ?? activeSession?.SessionCode ?? table.Code;
            var qrPayload = _vietQrService.GenerateDynamicVietQr(invoiceRef, totalAmount);
            dynamicVietQrText = qrPayload.QrString;
            vietQrQuickLink = qrPayload.QuickLink;
        }

        bool canOrder = table.Status is TableStatus.Available or TableStatus.Occupied or TableStatus.BillingRequested;

        return new GuestTableSessionDto(
            TableId: table.Id,
            TableCode: table.Code,
            TableName: table.Name,
            AreaName: area?.Name ?? "Khu Vực",
            RoomName: room?.Name,
            Status: table.Status,
            CanOrder: canOrder,
            SessionId: activeSession?.Id,
            SessionCode: activeSession?.SessionCode,
            OrderedItems: orderedItems,
            TotalBillAmount: totalAmount,
            DynamicVietQrText: dynamicVietQrText,
            VietQrQuickLink: vietQrQuickLink
        );
    }

    public async Task<GuestOrderResultDto> SubmitGuestOrderAsync(GuestOrderRequestDto request, CancellationToken ct = default)
    {
        if (!_qrTokenService.ValidateToken(request.TableId, request.Token))
        {
            throw new DomainValidationException("Mã QR bảo mật của bàn không hợp lệ hoặc đã bị thay đổi.");
        }

        if (request.Items == null || request.Items.Count == 0)
        {
            throw new DomainValidationException("Vui lòng chọn ít nhất một món ăn trước khi gửi yêu cầu.");
        }

        var table = await _dbContext.RestaurantTables
            .FirstOrDefaultAsync(t => t.Id == request.TableId, ct)
            ?? throw new DomainNotFoundException("Không tìm thấy bàn yêu cầu.");

        if (table.Status == TableStatus.CleaningNeeded)
        {
            throw new DomainValidationException("Bàn đang trong quá trình dọn dẹp vệ sinh. Vui lòng liên hệ nhân viên phục vụ.");
        }

        if (table.Status == TableStatus.Reserved)
        {
            throw new DomainValidationException("Bàn đã được khách đặt trước. Vui lòng liên hệ nhân viên để được xếp bàn phù hợp.");
        }

        // Idempotency check: Return existing round if key exists
        if (!string.IsNullOrWhiteSpace(request.IdempotencyKey))
        {
            var existingRound = await _dbContext.OrderRounds
                .Include(r => r.Items)
                .FirstOrDefaultAsync(r => r.IdempotencyKey == request.IdempotencyKey.Trim(), ct);

            if (existingRound != null)
            {
                var existingOrder = await _dbContext.Orders.FirstOrDefaultAsync(o => o.Id == existingRound.OrderId, ct);
                var existingRoundTotal = existingRound.Items.Sum(i => i.UnitPrice * i.Quantity);
                return new GuestOrderResultDto(
                    Success: true,
                    Message: $"Yêu cầu gọi món đợt {existingRound.RoundNumber} đã được ghi nhận trước đó.",
                    RoundNumber: existingRound.RoundNumber,
                    TotalItemsOrdered: existingRound.Items.Sum(i => i.Quantity),
                    RoundTotalAmount: existingRoundTotal,
                    NewSessionTotal: existingOrder?.TotalAmount ?? existingRoundTotal
                );
            }
        }

        var requestedMenuItemIds = request.Items.Select(i => i.MenuItemId).Distinct().ToList();
        var menuItems = await _dbContext.MenuItems
            .Include(m => m.Variants)
            .Where(m => requestedMenuItemIds.Contains(m.Id))
            .ToListAsync(ct);

        var requestedModifierOptionIds = request.Items
            .SelectMany(i => i.ModifierOptionIds ?? [])
            .Distinct()
            .ToList();

        var modifierOptions = requestedModifierOptionIds.Count > 0
            ? await _dbContext.ModifierOptions.Where(o => requestedModifierOptionIds.Contains(o.Id)).ToListAsync(ct)
            : [];

        var countableInventory = await _dbContext.InventoryItems
            .Where(inv => requestedMenuItemIds.Contains(inv.MenuItemId ?? Guid.Empty) && inv.BranchId == table.BranchId)
            .ToListAsync(ct);

        foreach (var reqItem in request.Items)
        {
            var item = menuItems.FirstOrDefault(m => m.Id == reqItem.MenuItemId)
                ?? throw new DomainNotFoundException("Không tìm thấy món ăn trong thực đơn.");

            if (!item.IsAvailable)
            {
                throw new DomainValidationException($"Món '{item.Name}' hiện tại tạm hết (86). Quý khách vui lòng chọn món khác.");
            }

            var invItem = countableInventory.FirstOrDefault(inv => inv.MenuItemId == item.Id);
            if (invItem != null && item.IsCountable)
            {
                int available = invItem.AvailableStock;
                if (available < reqItem.Quantity)
                {
                    throw new DomainValidationException($"Món '{item.Name}' chỉ còn lại {Math.Max(0, available)} phần, không đủ số lượng yêu cầu ({reqItem.Quantity}).");
                }
            }
        }

        var room = table.DiningRoomId.HasValue
            ? await _dbContext.DiningRooms.FirstOrDefaultAsync(r => r.Id == table.DiningRoomId.Value, ct)
            : null;

        return await _dbContext.ExecuteInTransactionAsync(async () =>
        {
            // 1. Resolve or Create DiningSession
            var session = await _dbContext.DiningSessions
                .FirstOrDefaultAsync(s => s.TableId == table.Id && s.Status == DiningSessionStatus.Active, ct);

            if (session == null)
            {
                session = new DiningSession(table.BranchId, table.Id, $"SES-GUEST-{DateTime.UtcNow:yyyyMMddHHmmss}-{table.Code}", guestCount: 2);
                _dbContext.DiningSessions.Add(session);
                table.Occupy();

                _dbContext.OutboxMessages.Add(new OutboxMessage(
                    "TableStatusChanged",
                    JsonSerializer.Serialize(new { TableId = table.Id, Status = TableStatus.Occupied, TableCode = table.Code })
                ));
            }

            // 2. Resolve or Create Order
            var order = await _dbContext.Orders
                .Include(o => o.Rounds)
                .FirstOrDefaultAsync(o => o.DiningSessionId == session.Id && !o.IsSettled && o.Status != OrderStatus.Cancelled, ct);

            if (order == null)
            {
                var businessDate = DateTime.UtcNow.TimeOfDay < TimeSpan.FromHours(6)
                    ? DateTime.UtcNow.Date.AddDays(-1)
                    : DateTime.UtcNow.Date;

                order = new Order(table.BranchId, session.Id, $"ORD-GUEST-{DateTime.UtcNow:yyyyMMddHHmmss}-{table.Code}", businessDate);
                _dbContext.Orders.Add(order);
            }

            // 3. Create OrderRound
            int roundNumber = (order.Rounds.Count > 0 ? order.Rounds.Max(r => r.RoundNumber) : 0) + 1;
            string idempotencyKey = string.IsNullOrWhiteSpace(request.IdempotencyKey)
                ? $"IDEM-GUEST-{Guid.NewGuid():N}"
                : request.IdempotencyKey.Trim();

            string customerNoteText = string.IsNullOrWhiteSpace(request.CustomerNote) ? "" : $" | {request.CustomerNote.Trim()}";
            string roundNote = $"[KHÁCH TỰ GỌI ĐỢT {roundNumber}]{customerNoteText}";

            var waiterStaff = await _dbContext.StaffMembers.FirstOrDefaultAsync(s => s.BranchId == table.BranchId && s.Role == StaffRole.Waiter, ct);
            var staffId = waiterStaff?.Id ?? Guid.Parse("99999999-9999-9999-9999-999999999999");

            var round = new OrderRound(order.Id, roundNumber, staffId, idempotencyKey, roundNote);

            decimal roundAmount = 0;
            var createdOrderItems = new List<OrderItem>();

            foreach (var reqItem in request.Items)
            {
                var menuItem = menuItems.First(m => m.Id == reqItem.MenuItemId);
                decimal unitPrice = menuItem.BasePrice;

                if (reqItem.MenuItemVariantId.HasValue)
                {
                    var variant = menuItem.Variants.FirstOrDefault(v => v.Id == reqItem.MenuItemVariantId.Value);
                    if (variant != null)
                    {
                        unitPrice += variant.PriceAdjustment;
                    }
                }

                // Two-Phase Inventory Lock (D2, D5)
                var invItem = countableInventory.FirstOrDefault(inv => inv.MenuItemId == menuItem.Id);
                if (invItem != null && menuItem.IsCountable)
                {
                    int prevPhysical = invItem.PhysicalStock;
                    invItem.ReserveStock(reqItem.Quantity);
                    _dbContext.InventoryTransactions.Add(new InventoryTransaction(
                        invItem.BranchId,
                        invItem.Id,
                        invItem.ItemCode,
                        invItem.ItemName,
                        InventoryTransactionType.Reservation,
                        reqItem.Quantity,
                        prevPhysical,
                        invItem.PhysicalStock,
                        $"Tạm giữ {reqItem.Quantity} suất khi khách tự gọi món bàn {table.Code}",
                        "Cổng Khách Hàng QR"
                    ));
                }

                var orderItem = new OrderItem(
                    orderRoundId: round.Id,
                    menuItemId: menuItem.Id,
                    itemName: menuItem.Name,
                    unitPrice: unitPrice,
                    quantity: reqItem.Quantity,
                    menuItemVariantId: reqItem.MenuItemVariantId,
                    kitchenNote: reqItem.KitchenNote,
                    directFOHPick: menuItem.DirectFOHPick,
                    kitchenStationId: menuItem.KitchenStationId
                );

                if (reqItem.ModifierOptionIds != null && reqItem.ModifierOptionIds.Count > 0)
                {
                    foreach (var optId in reqItem.ModifierOptionIds)
                    {
                        var opt = modifierOptions.FirstOrDefault(o => o.Id == optId);
                        if (opt != null)
                        {
                            orderItem.AddModifier(new OrderItemModifier(orderItem.Id, opt.Id, opt.Name, opt.AdditionalPrice));
                        }
                    }
                }

                round.AddItem(orderItem);
                _dbContext.OrderItems.Add(orderItem);
                createdOrderItems.Add(orderItem);

                roundAmount += (unitPrice + orderItem.Modifiers.Sum(m => m.PriceAdjustment)) * reqItem.Quantity;
            }

            order.AddRound(round);
            _dbContext.OrderRounds.Add(round);

            // Recalculate order totals
            order.RecalculateTotals(room?.SurchargeRate ?? 0, room?.SurchargeFixedAmount ?? 0, 0);
            session.AddAmount(roundAmount);

            // 4. Generate Kitchen Tickets by Station
            var kitchenItems = createdOrderItems.Where(i => !i.DirectFOHPick && i.KitchenStationId.HasValue).ToList();
            var groupedByStation = kitchenItems.GroupBy(i => i.KitchenStationId!.Value);

            foreach (var group in groupedByStation)
            {
                var stationId = group.Key;
                var stationItems = group.ToList();

                var ticketItemsPayload = stationItems.Select(i => new
                {
                    ItemName = i.ItemName,
                    Quantity = i.Quantity,
                    KitchenNote = i.KitchenNote,
                    Modifiers = i.Modifiers.Select(m => m.OptionName).ToList()
                }).ToList();

                string itemsJson = JsonSerializer.Serialize(ticketItemsPayload);
                string ticketNumber = $"TK-GUEST-{DateTime.UtcNow:yyyyMMddHHmmss}-{table.Code}-R{round.RoundNumber}";

                var ticket = new KitchenTicket(
                    kitchenStationId: stationId,
                    orderRoundId: round.Id,
                    ticketNumber: ticketNumber,
                    itemsJson: itemsJson,
                    notes: $"[KHÁCH TỰ GỌI] {table.Code} (Đợt {round.RoundNumber})",
                    tableCode: table.Code,
                    isHighPriority: false
                );

                _dbContext.KitchenTickets.Add(ticket);
            }

            // 5. Broadcast Outbox Messages for SignalR
            _dbContext.OutboxMessages.Add(new OutboxMessage(
                "GuestOrderPlaced",
                JsonSerializer.Serialize(new
                {
                    TableId = table.Id,
                    TableCode = table.Code,
                    RoundNumber = round.RoundNumber,
                    ItemCount = createdOrderItems.Sum(i => i.Quantity),
                    TotalAmount = roundAmount
                })
            ));

            _dbContext.OutboxMessages.Add(new OutboxMessage(
                "OrderFired",
                JsonSerializer.Serialize(new
                {
                    OrderId = order.Id,
                    TableId = table.Id,
                    TableCode = table.Code,
                    RoundNumber = round.RoundNumber,
                    FiredAt = round.FiredAt
                })
            ));

            await _dbContext.SaveChangesAsync(ct);

            return new GuestOrderResultDto(
                Success: true,
                Message: $"Đã gửi yêu cầu gọi món đợt {round.RoundNumber} thành công! Bếp đang tiếp nhận món.",
                RoundNumber: round.RoundNumber,
                TotalItemsOrdered: createdOrderItems.Sum(i => i.Quantity),
                RoundTotalAmount: roundAmount,
                NewSessionTotal: order.TotalAmount
            );
        }, ct);
    }

    public async Task<bool> RequestBillAsync(Guid tableId, string? token, CancellationToken ct = default)
    {
        if (!_qrTokenService.ValidateToken(tableId, token))
        {
            throw new DomainValidationException("Mã QR bảo mật của bàn không hợp lệ hoặc đã bị thay đổi.");
        }

        var table = await _dbContext.RestaurantTables
            .FirstOrDefaultAsync(t => t.Id == tableId, ct)
            ?? throw new DomainNotFoundException("Không tìm thấy bàn yêu cầu.");

        table.RequestBilling();

        _dbContext.OutboxMessages.Add(new OutboxMessage(
            "TableStatusChanged",
            JsonSerializer.Serialize(new
            {
                TableId = table.Id,
                Status = (int)TableStatus.BillingRequested,
                StatusName = TableStatus.BillingRequested.ToString(),
                TableCode = table.Code
            })
        ));

        await _dbContext.SaveChangesAsync(ct);
        return true;
    }

    public async Task<TableQrInfoDto> GetTableQrInfoAsync(Guid tableId, string baseUrl, CancellationToken ct = default)
    {
        var table = await _dbContext.RestaurantTables
            .FirstOrDefaultAsync(t => t.Id == tableId, ct)
            ?? throw new DomainNotFoundException("Không tìm thấy bàn yêu cầu.");

        var area = await _dbContext.DiningAreas.FirstOrDefaultAsync(a => a.Id == table.DiningAreaId, ct);

        var token = _qrTokenService.GenerateToken(tableId);
        var qrUrl = _qrTokenService.BuildGuestOrderUrl(baseUrl, tableId);

        return new TableQrInfoDto(
            TableId: table.Id,
            TableCode: table.Code,
            TableName: table.Name,
            AreaName: area?.Name ?? "Khu Vực",
            Token: token,
            QrUrl: qrUrl
        );
    }
}
