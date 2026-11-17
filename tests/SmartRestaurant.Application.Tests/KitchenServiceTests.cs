namespace SmartRestaurant.Application.Tests;

using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SmartRestaurant.Application.DTOs;
using SmartRestaurant.Application.Services;
using SmartRestaurant.Domain.Entities;
using SmartRestaurant.Domain.Enums;
using SmartRestaurant.Infrastructure.Data;
using Xunit;

public class KitchenServiceTests
{
    private readonly Guid _branchId = Guid.NewGuid();
    private readonly Guid _managerId = Guid.NewGuid();
    private readonly Guid _staffId = Guid.NewGuid();

    private (AppDbContext db, KitchenService service, KitchenStation stationHot, KitchenStation stationCold, RestaurantTable table, MenuItem steakItem) CreateTestFixture(string dbName)
    {
        var db = TestDbContextFactory.Create(dbName);
        var service = new KitchenService(db);

        var area = new DiningArea(_branchId, "Tầng 1 - Phòng Lạnh", AreaType.Indoor, 1);
        db.DiningAreas.Add(area);

        var table = new RestaurantTable(_branchId, area.Id, "VIP-01", "Bàn VIP 1", 6);
        db.RestaurantTables.Add(table);

        var manager = new StaffMember(_branchId, "Trần Bếp Trưởng", "chef01", "chef@restaurant.vn", StaffRole.Chef, "9999");
        db.StaffMembers.Add(manager);

        var stationHot = new KitchenStation(_branchId, "BEP-NONG", "Bếp Nóng Chuyên Trách", PrepStation.HotKitchen);
        var stationCold = new KitchenStation(_branchId, "BEP-LANH", "Bếp Lạnh & Salad", PrepStation.ColdKitchen);
        db.KitchenStations.AddRange(stationHot, stationCold);

        var cat = new MenuCategory(_branchId, "Món Chính", "Món ăn cao cấp", 1);
        db.MenuCategories.Add(cat);

        var steakItem = new MenuItem(_branchId, cat.Id, "BO-01", "Bò Bít Tết Úc Sốt Tiêu", 250000m, directFOHPick: false);
        steakItem.AssignToStation(stationHot.Id, PrepStation.HotKitchen);
        db.MenuItems.Add(steakItem);

        db.SaveChanges();

        return (db, service, stationHot, stationCold, table, steakItem);
    }

    [Fact]
    public async Task GetStationsSummaryAsync_ShouldReturnActiveStationsAndCounts()
    {
        var (db, service, stationHot, stationCold, table, _) = CreateTestFixture(Guid.NewGuid().ToString());

        var ticket1 = new KitchenTicket(stationHot.Id, Guid.NewGuid(), "TK-01", "[]", "Ghi chú 1", table.Code, false);
        var ticket2 = new KitchenTicket(stationHot.Id, Guid.NewGuid(), "TK-02", "[]", "Ghi chú 2", table.Code, false);
        ticket2.StartPreparation();

        var ticket3 = new KitchenTicket(stationCold.Id, Guid.NewGuid(), "TK-03", "[]", "Ghi chú 3", table.Code, false);

        db.KitchenTickets.AddRange(ticket1, ticket2, ticket3);
        await db.SaveChangesAsync();

        var summaries = await service.GetStationsSummaryAsync(_branchId);

        summaries.Should().HaveCount(2);
        var hotSummary = summaries.First(s => s.StationId == stationHot.Id);
        hotSummary.PendingCount.Should().Be(1);
        hotSummary.InPrepCount.Should().Be(1);
        hotSummary.TotalActiveCount.Should().Be(2);

        var coldSummary = summaries.First(s => s.StationId == stationCold.Id);
        coldSummary.PendingCount.Should().Be(1);
        coldSummary.InPrepCount.Should().Be(0);
        coldSummary.TotalActiveCount.Should().Be(1);
    }

    [Fact]
    public async Task GetActiveTicketsAsync_ShouldFilterByStation_AndSortHighPriorityFirst()
    {
        var (db, service, stationHot, _, table, steakItem) = CreateTestFixture(Guid.NewGuid().ToString());

        var session = new DiningSession(_branchId, table.Id, "SS-01", 4);
        db.DiningSessions.Add(session);

        var order = new Order(_branchId, session.Id, "OD-TEST-01", DateTime.UtcNow);
        db.Orders.Add(order);

        var round = new OrderRound(order.Id, 1, _staffId, Guid.NewGuid().ToString());
        db.OrderRounds.Add(round);

        var itemNormal = new OrderItem(round.Id, steakItem.Id, steakItem.Name, steakItem.BasePrice, 1, null, null, false, stationHot.Id);
        db.OrderItems.Add(itemNormal);

        var ticketNormal = new KitchenTicket(stationHot.Id, round.Id, "TK-NORMAL", "[]", "Bình thường", table.Code, false);
        var ticketPriority = new KitchenTicket(stationHot.Id, round.Id, "TK-VIP-PRIORITY", "[]", "Ưu tiên gấp", table.Code, true);

        db.KitchenTickets.AddRange(ticketNormal, ticketPriority);
        await db.SaveChangesAsync();

        var tickets = await service.GetActiveTicketsAsync(_branchId, stationHot.Id);

        tickets.Should().HaveCount(2);
        tickets[0].TicketNumber.Should().Be("TK-VIP-PRIORITY");
        tickets[0].IsHighPriority.Should().BeTrue();
        tickets[1].TicketNumber.Should().Be("TK-NORMAL");
        tickets[1].IsHighPriority.Should().BeFalse();
    }

    [Fact]
    public async Task GetAggregatedQueueAsync_ShouldAggregatePendingDishesAcrossTables()
    {
        var (db, service, stationHot, _, table, steakItem) = CreateTestFixture(Guid.NewGuid().ToString());

        var session = new DiningSession(_branchId, table.Id, "SS-AGG-01", 4);
        db.DiningSessions.Add(session);

        var order = new Order(_branchId, session.Id, "OD-AGG-01", DateTime.UtcNow);
        db.Orders.Add(order);

        var round1 = new OrderRound(order.Id, 1, _staffId, Guid.NewGuid().ToString());
        var round2 = new OrderRound(order.Id, 2, _staffId, Guid.NewGuid().ToString());
        db.OrderRounds.AddRange(round1, round2);

        var item1 = new OrderItem(round1.Id, steakItem.Id, steakItem.Name, steakItem.BasePrice, 2, null, null, false, stationHot.Id);
        var item2 = new OrderItem(round2.Id, steakItem.Id, steakItem.Name, steakItem.BasePrice, 3, null, null, false, stationHot.Id);
        db.OrderItems.AddRange(item1, item2);

        var ticket1 = new KitchenTicket(stationHot.Id, round1.Id, "TK-AGG-1", "[]", "", table.Code, false);
        var ticket2 = new KitchenTicket(stationHot.Id, round2.Id, "TK-AGG-2", "[]", "", table.Code, false);
        db.KitchenTickets.AddRange(ticket1, ticket2);
        await db.SaveChangesAsync();

        var queue = await service.GetAggregatedQueueAsync(_branchId, stationHot.Id);

        queue.Should().HaveCount(1);
        queue[0].ItemName.Should().Be("Bò Bít Tết Úc Sốt Tiêu");
        queue[0].TotalPending.Should().Be(5);
    }

    [Fact]
    public async Task UpdateTicketStatusAsync_ShouldUpdateTicketAndCascadeToOrderItems()
    {
        var (db, service, stationHot, _, table, steakItem) = CreateTestFixture(Guid.NewGuid().ToString());

        var round = new OrderRound(Guid.NewGuid(), 1, _staffId, Guid.NewGuid().ToString());
        db.OrderRounds.Add(round);

        var item = new OrderItem(round.Id, steakItem.Id, steakItem.Name, steakItem.BasePrice, 2, null, null, false, stationHot.Id);
        db.OrderItems.Add(item);

        var ticket = new KitchenTicket(stationHot.Id, round.Id, "TK-CASCADE", "[]", "", table.Code, false);
        db.KitchenTickets.Add(ticket);
        await db.SaveChangesAsync();

        var result = await service.UpdateTicketStatusAsync(new UpdateTicketStatusRequest(ticket.Id, KitchenTicketStatus.InPreparation, _staffId));

        result.Should().BeTrue();
        var updatedTicket = await db.KitchenTickets.FirstAsync(t => t.Id == ticket.Id);
        updatedTicket.Status.Should().Be(KitchenTicketStatus.InPreparation);

        var updatedItem = await db.OrderItems.FirstAsync(i => i.Id == item.Id);
        updatedItem.Status.Should().Be(OrderItemStatus.Cooking);

        var outbox = await db.OutboxMessages.FirstOrDefaultAsync(m => m.EventType == "KitchenTicketUpdated");
        outbox.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateOrderItemStatusAsync_ShouldAutoCompleteTicketWhenAllItemsReady()
    {
        var (db, service, stationHot, _, table, steakItem) = CreateTestFixture(Guid.NewGuid().ToString());

        var round = new OrderRound(Guid.NewGuid(), 1, _staffId, Guid.NewGuid().ToString());
        db.OrderRounds.Add(round);

        var item = new OrderItem(round.Id, steakItem.Id, steakItem.Name, steakItem.BasePrice, 1, null, null, false, stationHot.Id);
        db.OrderItems.Add(item);

        var ticket = new KitchenTicket(stationHot.Id, round.Id, "TK-AUTOCOMP", "[]", "", table.Code, false);
        db.KitchenTickets.Add(ticket);
        await db.SaveChangesAsync();

        var result = await service.UpdateOrderItemStatusAsync(new UpdateItemStatusRequest(item.Id, OrderItemStatus.ReadyToServe, _staffId));

        result.Should().BeTrue();
        var updatedTicket = await db.KitchenTickets.FirstAsync(t => t.Id == ticket.Id);
        updatedTicket.Status.Should().Be(KitchenTicketStatus.Completed);

        var outbox = await db.OutboxMessages.FirstOrDefaultAsync(m => m.EventType == "OrderItemStatusChanged");
        outbox.Should().NotBeNull();
    }

    [Fact]
    public async Task RemakeDishAsync_ShouldCreateWasteLog_AndHighPriorityTicket_WithZeroCustomerCharge()
    {
        var (db, service, stationHot, _, table, steakItem) = CreateTestFixture(Guid.NewGuid().ToString());

        var session = new DiningSession(_branchId, table.Id, "SS-REMAKE-01", 2);
        db.DiningSessions.Add(session);

        var order = new Order(_branchId, session.Id, "OD-REMAKE-01", DateTime.UtcNow);
        db.Orders.Add(order);

        var round = new OrderRound(order.Id, 1, _staffId, Guid.NewGuid().ToString());
        db.OrderRounds.Add(round);

        var item = new OrderItem(round.Id, steakItem.Id, steakItem.Name, steakItem.BasePrice, 1, null, null, false, stationHot.Id);
        db.OrderItems.Add(item);
        await db.SaveChangesAsync();

        var remakeTicket = await service.RemakeDishAsync(new RemakeDishRequest(
            OrderItemId: item.Id,
            Reason: "Thịt bò bị dai quá, khách yêu cầu làm lại tái vừa",
            ManagerId: _managerId
        ));

        remakeTicket.Should().NotBeNull();
        remakeTicket.IsHighPriority.Should().BeTrue();
        remakeTicket.Items[0].IsRemake.Should().BeTrue();

        var wasteLog = await db.KitchenWasteLogs.FirstOrDefaultAsync();
        wasteLog.Should().NotBeNull();
        wasteLog!.MenuItemName.Should().Be(steakItem.Name);
        wasteLog.EstimatedLoss.Should().Be(steakItem.BasePrice);
        wasteLog.Reason.Should().Contain("LÀM LẠI MÓN LỖI BẾP");

        var updatedItem = await db.OrderItems.FirstAsync(i => i.Id == item.Id);
        updatedItem.IsRemake.Should().BeTrue();
        updatedItem.RemakeReason.Should().Contain("khách yêu cầu làm lại tái vừa");
    }

    [Fact]
    public async Task ToggleItem86Async_ShouldToggleMenuItemAvailabilityAndEnqueueOutbox()
    {
        var (db, service, _, _, _, steakItem) = CreateTestFixture(Guid.NewGuid().ToString());

        steakItem.IsAvailable.Should().BeTrue();

        // 86 the item (sold out)
        var resultOff = await service.ToggleItem86Async(new Toggle86Request(steakItem.Id, false, _staffId));
        resultOff.Should().BeTrue();

        var updatedItemOff = await db.MenuItems.FirstAsync(m => m.Id == steakItem.Id);
        updatedItemOff.IsAvailable.Should().BeFalse();

        // Bring back into stock
        var resultOn = await service.ToggleItem86Async(new Toggle86Request(steakItem.Id, true, _staffId));
        resultOn.Should().BeTrue();

        var updatedItemOn = await db.MenuItems.FirstAsync(m => m.Id == steakItem.Id);
        updatedItemOn.IsAvailable.Should().BeTrue();

        var outboxList = await db.OutboxMessages.Where(m => m.EventType == "Item86Toggled").ToListAsync();
        outboxList.Should().HaveCount(2);
    }
}
