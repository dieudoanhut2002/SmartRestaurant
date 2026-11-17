namespace SmartRestaurant.Application.Tests;

using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SmartRestaurant.Application.DTOs;
using SmartRestaurant.Application.Services;
using SmartRestaurant.Domain.Entities;
using SmartRestaurant.Domain.Enums;
using SmartRestaurant.Infrastructure.Data;
using Xunit;

public class PosOrderServiceTests
{
    private readonly Guid _branchId = Guid.NewGuid();
    private readonly Guid _staffId = Guid.NewGuid();

    private (AppDbContext db, PosOrderService service, RestaurantTable table, MenuItem countableItem, MenuItem fohItem, KitchenStation station) CreateTestFixture(string dbName)
    {
        var db = TestDbContextFactory.Create(dbName);
        var service = new PosOrderService(db);

        var area = new DiningArea(_branchId, "Khu Ngoài Trời", AreaType.Outdoor, 1);
        db.DiningAreas.Add(area);

        var table = new RestaurantTable(_branchId, area.Id, "NT-01", "Bàn Ngoài Trời 1", 4);
        db.RestaurantTables.Add(table);

        var staff = new StaffMember(_branchId, "Nguyễn Văn Phục Vụ", "waiter01", "waiter@restaurant.vn", StaffRole.Waiter, "1234");
        db.StaffMembers.Add(staff);

        var station = new KitchenStation(_branchId, "BEP-NONG", "Bếp Nóng Chuyên Trách", PrepStation.HotKitchen);
        db.KitchenStations.Add(station);

        var cat = new MenuCategory(_branchId, "Đồ Uống & Món Nóng", "Món ngon", 1);
        db.MenuCategories.Add(cat);

        // Countable item (Bia lon) with inventory tracking
        var countableItem = new MenuItem(_branchId, cat.Id, "BIA-01", "Bia Sài Gòn Special", 22000m, directFOHPick: false);
        db.MenuItems.Add(countableItem);

        var inv = new InventoryItem(_branchId, "INV-BIA-01", "Bia Sài Gòn Special", "Đồ Uống", "Lon", initialPhysicalStock: 50, minimumThreshold: 10, menuItemId: countableItem.Id);
        db.InventoryItems.Add(inv);

        // DirectFOHPick item (Nước suối)
        var fohItem = new MenuItem(_branchId, cat.Id, "SUOI-01", "Nước suối Aquafina", 15000m, directFOHPick: true);
        db.MenuItems.Add(fohItem);

        db.SaveChanges();

        return (db, service, table, countableItem, fohItem, station);
    }

    [Fact]
    public async Task OpenDiningSessionAsync_ShouldOccupyTable_AndCreateSessionWithOrder()
    {
        // Arrange
        var (db, service, table, _, _, _) = CreateTestFixture(nameof(OpenDiningSessionAsync_ShouldOccupyTable_AndCreateSessionWithOrder));
        var req = new OpenDiningSessionRequest(table.Id, 4, _staffId);

        // Act
        var result = await service.OpenDiningSessionAsync(req);

        // Assert
        result.Should().NotBeNull();
        result.GuestCount.Should().Be(4);
        result.Status.Should().Be(DiningSessionStatus.Active);
        result.ActiveOrderId.Should().NotBeNull();

        var updatedTable = await db.RestaurantTables.FindAsync(table.Id);
        updatedTable!.Status.Should().Be(TableStatus.Occupied);

        var session = await db.DiningSessions.FirstOrDefaultAsync(s => s.TableId == table.Id);
        session.Should().NotBeNull();
        session!.GuestCount.Should().Be(4);

        var outbox = await db.OutboxMessages.FirstOrDefaultAsync(m => m.EventType == "TableStatusChanged");
        outbox.Should().NotBeNull();
    }

    [Fact]
    public async Task FireOrderAsync_CountableItem_ShouldIncreaseReservedStockOnly()
    {
        // Arrange
        var (db, service, table, countableItem, _, _) = CreateTestFixture(nameof(FireOrderAsync_CountableItem_ShouldIncreaseReservedStockOnly));
        await service.OpenDiningSessionAsync(new OpenDiningSessionRequest(table.Id, 2, _staffId));

        var fireReq = new FireOrderRequest(
            TableId: table.Id,
            OrderId: null,
            IdempotencyKey: "IDEM-TEST-RESERVE-01",
            StaffId: _staffId,
            Note: "Bàn 1 gọi 5 lon bia",
            Items: new List<FireOrderItemDraftDto>
            {
                new(countableItem.Id, null, countableItem.Name, countableItem.BasePrice, 5, null, null)
            }
        );

        // Act
        var round = await service.FireOrderAsync(fireReq);

        // Assert
        round.Should().NotBeNull();
        round.RoundNumber.Should().Be(1);
        round.Items.Should().HaveCount(1);

        var inv = await db.InventoryItems.FirstAsync(i => i.MenuItemId == countableItem.Id);
        inv.PhysicalStock.Should().Be(50, "PhysicalStock không được giảm khi mới báo bếp (Two-Phase Lock D2)");
        inv.ReservedStock.Should().Be(5, "ReservedStock phải tăng lên đúng 5");
        inv.AvailableStock.Should().Be(45, "AvailableStock = Physical (50) - Reserved (5) = 45 (INV-03 / D2)");
        inv.ConsumedStock.Should().Be(0, "ConsumedStock không được thay đổi ở pha báo bếp");
    }

    [Fact]
    public async Task FireOrderAsync_DirectFOHPick_ShouldSetReadyToServe_AndNotCreateKitchenTicket()
    {
        // Arrange
        var (db, service, table, _, fohItem, _) = CreateTestFixture(nameof(FireOrderAsync_DirectFOHPick_ShouldSetReadyToServe_AndNotCreateKitchenTicket));
        await service.OpenDiningSessionAsync(new OpenDiningSessionRequest(table.Id, 2, _staffId));

        var fireReq = new FireOrderRequest(
            TableId: table.Id,
            OrderId: null,
            IdempotencyKey: "IDEM-TEST-FOH-01",
            StaffId: _staffId,
            Note: "Lấy nước suối trực tiếp tại tủ sảnh",
            Items: new List<FireOrderItemDraftDto>
            {
                new(fohItem.Id, null, fohItem.Name, fohItem.BasePrice, 2, null, null)
            }
        );

        // Act
        var round = await service.FireOrderAsync(fireReq);

        // Assert
        round.Should().NotBeNull();
        var item = round.Items.First();
        item.DirectFOHPick.Should().BeTrue();
        item.Status.Should().Be(OrderItemStatus.ReadyToServe, "Mặt hàng DirectFOHPick lập tức sẵn sàng phục vụ mà không cần chờ bếp nấu (PDR-012)");

        var tickets = await db.KitchenTickets.ToListAsync();
        tickets.Should().BeEmpty("Món phục vụ trực tiếp sảnh DirectFOHPick không được tạo vé bếp KDS (PDR-012)");
    }

    [Fact]
    public async Task FireOrderAsync_DuplicateIdempotencyKey_ShouldReturnExistingRoundWithoutDuplication()
    {
        // Arrange
        var (db, service, table, countableItem, _, _) = CreateTestFixture(nameof(FireOrderAsync_DuplicateIdempotencyKey_ShouldReturnExistingRoundWithoutDuplication));
        await service.OpenDiningSessionAsync(new OpenDiningSessionRequest(table.Id, 2, _staffId));

        var fireReq = new FireOrderRequest(
            TableId: table.Id,
            OrderId: null,
            IdempotencyKey: "IDEM-DUPLICATE-CHECK",
            StaffId: _staffId,
            Note: "Lần 1",
            Items: new List<FireOrderItemDraftDto>
            {
                new(countableItem.Id, null, countableItem.Name, countableItem.BasePrice, 3, null, null)
            }
        );

        // Act - Call twice with same IdempotencyKey
        var round1 = await service.FireOrderAsync(fireReq);
        var round2 = await service.FireOrderAsync(fireReq);

        // Assert
        round1.RoundId.Should().Be(round2.RoundId);
        round1.IdempotencyKey.Should().Be(round2.IdempotencyKey);

        var roundsCount = await db.OrderRounds.CountAsync(r => r.IdempotencyKey == "IDEM-DUPLICATE-CHECK");
        roundsCount.Should().Be(1, "Chống gửi trùng vé: Chỉ được tồn tại đúng 1 OrderRound trong CSDL");

        var inv = await db.InventoryItems.FirstAsync(i => i.MenuItemId == countableItem.Id);
        inv.ReservedStock.Should().Be(3, "Tồn kho tạm giữ chỉ được trừ 1 lần, không được nhân đôi khi request gửi lặp");
    }

    [Fact]
    public async Task FireOrderAsync_WhenTableIsBillingRequested_ShouldAppendToCurrentUnpaidOrder()
    {
        // Arrange
        var (db, service, table, countableItem, _, _) = CreateTestFixture(nameof(FireOrderAsync_WhenTableIsBillingRequested_ShouldAppendToCurrentUnpaidOrder));
        await service.OpenDiningSessionAsync(new OpenDiningSessionRequest(table.Id, 2, _staffId));

        // Round 1
        await service.FireOrderAsync(new FireOrderRequest(
            table.Id, null, "IDEM-ROUND-1", _staffId, null,
            new List<FireOrderItemDraftDto> { new(countableItem.Id, null, countableItem.Name, countableItem.BasePrice, 2, null, null) }
        ));

        // Customer requests bill -> table transitions to BillingRequested
        await service.RequestBillAsync(new RequestBillRequest(table.Id, _staffId));

        var tableAfterBill = await db.RestaurantTables.FindAsync(table.Id);
        tableAfterBill!.Status.Should().Be(TableStatus.BillingRequested);

        // Act - Customer calls more items while in BillingRequested (D4 / BillingRequested Option 2)
        var round2 = await service.FireOrderAsync(new FireOrderRequest(
            table.Id, null, "IDEM-ROUND-2-ADDITIONAL", _staffId, null,
            new List<FireOrderItemDraftDto> { new(countableItem.Id, null, countableItem.Name, countableItem.BasePrice, 1, null, null) }
        ));

        // Assert
        round2.Should().NotBeNull();
        round2.RoundNumber.Should().Be(2);

        var detail = await service.GetTableOrderDetailAsync(table.Id);
        detail!.Rounds.Should().HaveCount(2, "Món mới phải được nối tiếp vào Order hiện tại của bàn chưa thanh toán (D4 / PDR-009 §2)");
        detail.TotalAmount.Should().Be(3 * 22000m);
    }

    [Fact]
    public async Task TransferTableAsync_ShouldMoveSessionAndResetOldTable()
    {
        // Arrange
        var (db, service, table1, _, _, _) = CreateTestFixture(nameof(TransferTableAsync_ShouldMoveSessionAndResetOldTable));
        var table2 = new RestaurantTable(_branchId, table1.DiningAreaId, "NT-02", "Bàn Ngoài Trời 2", 4);
        db.RestaurantTables.Add(table2);
        await db.SaveChangesAsync();

        await service.OpenDiningSessionAsync(new OpenDiningSessionRequest(table1.Id, 2, _staffId));

        // Act
        var success = await service.TransferTableAsync(new TransferTableRequest(table1.Id, table2.Id, _staffId));

        // Assert
        success.Should().BeTrue();

        var t1 = await db.RestaurantTables.FindAsync(table1.Id);
        t1!.Status.Should().Be(TableStatus.Available, "Bàn cũ phải giải phóng về Available");

        var t2 = await db.RestaurantTables.FindAsync(table2.Id);
        t2!.Status.Should().Be(TableStatus.Occupied, "Bàn mới phải chuyển sang Occupied");

        var session = await db.DiningSessions.FirstOrDefaultAsync(s => s.Status == DiningSessionStatus.Active);
        session!.TableId.Should().Be(table2.Id, "Phiên ăn phải gắn với ID bàn mới");
    }

    [Fact]
    public async Task FireOrderAsync_WithTransactionalExecutionStrategy_ShouldPersistRoundTicketsAndStockAtomically()
    {
        // Arrange
        var (db, service, table, countableItem, _, station) = CreateTestFixture(nameof(FireOrderAsync_WithTransactionalExecutionStrategy_ShouldPersistRoundTicketsAndStockAtomically));
        countableItem.AssignToStation(station.Id, PrepStation.HotKitchen);
        await db.SaveChangesAsync();

        await service.OpenDiningSessionAsync(new OpenDiningSessionRequest(table.Id, 2, _staffId));

        var fireReq = new FireOrderRequest(
            TableId: table.Id,
            OrderId: null,
            IdempotencyKey: "IDEM-TRANSACTION-TEST-01",
            StaffId: _staffId,
            Note: "Bàn gọi món qua transaction an toàn",
            Items: new List<FireOrderItemDraftDto>
            {
                new(countableItem.Id, null, countableItem.Name, countableItem.BasePrice, 2, "Giao nhanh", null)
            }
        );

        // Act
        var round = await service.FireOrderAsync(fireReq);

        // Assert
        round.Should().NotBeNull();
        round.RoundNumber.Should().Be(1);
        round.Items.Should().HaveCount(1);

        var tickets = await db.KitchenTickets.Where(t => t.OrderRoundId == round.RoundId).ToListAsync();
        tickets.Should().HaveCount(1, "Vé bếp phải được tạo thành công trong cùng transaction");
        tickets.First().TableCode.Should().Be(table.Code, "TableCode phải được gán chính xác trên vé bếp");

        var outbox = await db.OutboxMessages.FirstOrDefaultAsync(m => m.EventType == "OrderFired");
        outbox.Should().NotBeNull("Outbox message OrderFired phải được ghi lại đồng thời");
    }
}
