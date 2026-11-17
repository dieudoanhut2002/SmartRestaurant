namespace SmartRestaurant.Application.Tests;

using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SmartRestaurant.Application.DTOs;
using SmartRestaurant.Application.Services;
using SmartRestaurant.Domain.Entities;
using SmartRestaurant.Domain.Enums;
using SmartRestaurant.Domain.Exceptions;
using SmartRestaurant.Infrastructure.Data;
using Xunit;

public class GuestOrderServiceTests
{
    private readonly Guid _branchId = Guid.NewGuid();
    private readonly TableQrTokenService _tokenService = new();
    private readonly VietQrService _vietQrService = new();

    private (AppDbContext db, GuestOrderService service, RestaurantTable table, MenuItem hotItem, MenuItem countableItem) CreateTestFixture(string dbName)
    {
        var db = TestDbContextFactory.Create(dbName);
        var service = new GuestOrderService(db, _tokenService, _vietQrService);

        var area = new DiningArea(_branchId, "Tầng 1 Sân Vườn", AreaType.Outdoor, 1);
        db.DiningAreas.Add(area);

        var table = new RestaurantTable(_branchId, area.Id, "SV-01", "Bàn Sân Vườn 1", 4);
        db.RestaurantTables.Add(table);

        var station = new KitchenStation(_branchId, "KS-HOT", "Bếp Nóng", PrepStation.HotKitchen);
        db.KitchenStations.Add(station);

        var cat = new MenuCategory(_branchId, "Món Ăn & Đồ Uống", "Thực đơn chính", 1);
        db.MenuCategories.Add(cat);

        var hotItem = new MenuItem(_branchId, cat.Id, "BO-01", "Bò Lúc Lắc", 150000m, prepStation: PrepStation.HotKitchen);
        hotItem.AssignToStation(station.Id, PrepStation.HotKitchen);
        db.MenuItems.Add(hotItem);

        var countableItem = new MenuItem(_branchId, cat.Id, "BIA-01", "Bia Heineken", 30000m, directFOHPick: true, isCountable: true);
        db.MenuItems.Add(countableItem);

        var inv = new InventoryItem(_branchId, "INV-BIA", "Bia Heineken", "Đồ Uống", "Lon", initialPhysicalStock: 50, minimumThreshold: 10, menuItemId: countableItem.Id);
        db.InventoryItems.Add(inv);

        db.SaveChanges();

        return (db, service, table, hotItem, countableItem);
    }

    [Fact]
    public void TableQrTokenService_GenerateAndValidate_ShouldSucceedAndDetectTampering()
    {
        // Arrange
        var tableId = Guid.NewGuid();
        var otherTableId = Guid.NewGuid();

        // Act
        var token = _tokenService.GenerateToken(tableId);

        // Assert
        token.Should().NotBeNullOrWhiteSpace();
        _tokenService.ValidateToken(tableId, token).Should().BeTrue();

        // Tampering checks
        _tokenService.ValidateToken(otherTableId, token).Should().BeFalse();
        _tokenService.ValidateToken(tableId, token + "tampered").Should().BeFalse();
        _tokenService.ValidateToken(tableId, null).Should().BeFalse();
        _tokenService.ValidateToken(tableId, "").Should().BeFalse();
    }

    [Fact]
    public async Task GetTableSessionAsync_WithValidToken_ShouldReturnTableDetails()
    {
        // Arrange
        var (db, service, table, _, _) = CreateTestFixture(nameof(GetTableSessionAsync_WithValidToken_ShouldReturnTableDetails));
        var token = _tokenService.GenerateToken(table.Id);

        // Act
        var result = await service.GetTableSessionAsync(table.Id, token);

        // Assert
        result.Should().NotBeNull();
        result.TableCode.Should().Be("SV-01");
        result.AreaName.Should().Be("Tầng 1 Sân Vườn");
        result.CanOrder.Should().BeTrue();
        result.OrderedItems.Should().BeEmpty();
    }

    [Fact]
    public async Task GetTableSessionAsync_WithInvalidToken_ShouldThrowDomainValidationException()
    {
        // Arrange
        var (db, service, table, _, _) = CreateTestFixture(nameof(GetTableSessionAsync_WithInvalidToken_ShouldThrowDomainValidationException));

        // Act
        var act = async () => await service.GetTableSessionAsync(table.Id, "invalid-token");

        // Assert
        await act.Should().ThrowAsync<DomainValidationException>()
            .WithMessage("*Mã QR bảo mật*");
    }

    [Fact]
    public async Task SubmitGuestOrderAsync_OnAvailableTable_ShouldOccupyTable_CreateSessionAndKitchenTicket()
    {
        // Arrange
        var (db, service, table, hotItem, _) = CreateTestFixture(nameof(SubmitGuestOrderAsync_OnAvailableTable_ShouldOccupyTable_CreateSessionAndKitchenTicket));
        var token = _tokenService.GenerateToken(table.Id);

        var request = new GuestOrderRequestDto
        {
            TableId = table.Id,
            Token = token,
            CustomerNote = "Ít hành, cay vừa",
            Items =
            [
                new GuestOrderItemRequestDto
                {
                    MenuItemId = hotItem.Id,
                    Quantity = 2,
                    KitchenNote = "Làm nhanh giúp khách"
                }
            ]
        };

        // Act
        var result = await service.SubmitGuestOrderAsync(request);

        // Assert
        result.Success.Should().BeTrue();
        result.RoundNumber.Should().Be(1);
        result.TotalItemsOrdered.Should().Be(2);
        result.RoundTotalAmount.Should().Be(300000m);

        // Verify table was occupied
        var updatedTable = await db.RestaurantTables.FindAsync(table.Id);
        updatedTable!.Status.Should().Be(TableStatus.Occupied);

        // Verify session was created
        var session = await db.DiningSessions.FirstOrDefaultAsync(s => s.TableId == table.Id);
        session.Should().NotBeNull();
        session!.SessionCode.Should().StartWith("SES-GUEST-");

        // Verify order and round
        var order = await db.Orders.Include(o => o.Rounds).ThenInclude(r => r.Items).FirstOrDefaultAsync(o => o.DiningSessionId == session.Id);
        order.Should().NotBeNull();
        order!.Rounds.Should().HaveCount(1);
        order.TotalAmount.Should().Be(300000m);

        // Verify KitchenTicket created for hot dish
        var ticket = await db.KitchenTickets.FirstOrDefaultAsync(t => t.TableCode == "SV-01");
        ticket.Should().NotBeNull();
        ticket!.Notes.Should().Contain("[KHÁCH TỰ GỌI]");

        // Verify Outbox messages created for SignalR broadcast
        var outboxMessages = await db.OutboxMessages.ToListAsync();
        outboxMessages.Should().Contain(m => m.EventType == "GuestOrderPlaced");
        outboxMessages.Should().Contain(m => m.EventType == "OrderFired");
        outboxMessages.Should().Contain(m => m.EventType == "TableStatusChanged");
    }

    [Fact]
    public async Task SubmitGuestOrderAsync_WhenDishIs86_ShouldRejectWithValidationException()
    {
        // Arrange
        var (db, service, table, hotItem, _) = CreateTestFixture(nameof(SubmitGuestOrderAsync_WhenDishIs86_ShouldRejectWithValidationException));
        var token = _tokenService.GenerateToken(table.Id);

        // Mark dish as 86 (temporarily out of stock)
        hotItem.SetAvailability(false);
        await db.SaveChangesAsync();

        var request = new GuestOrderRequestDto
        {
            TableId = table.Id,
            Token = token,
            Items = [new GuestOrderItemRequestDto { MenuItemId = hotItem.Id, Quantity = 1 }]
        };

        // Act
        var act = async () => await service.SubmitGuestOrderAsync(request);

        // Assert
        await act.Should().ThrowAsync<DomainValidationException>()
            .WithMessage("*tạm hết (86)*");
    }

    [Fact]
    public async Task SubmitGuestOrderAsync_WithCountableItem_ShouldReserveInventory()
    {
        // Arrange
        var (db, service, table, _, countableItem) = CreateTestFixture(nameof(SubmitGuestOrderAsync_WithCountableItem_ShouldReserveInventory));
        var token = _tokenService.GenerateToken(table.Id);

        var request = new GuestOrderRequestDto
        {
            TableId = table.Id,
            Token = token,
            Items = [new GuestOrderItemRequestDto { MenuItemId = countableItem.Id, Quantity = 5 }]
        };

        // Act
        var result = await service.SubmitGuestOrderAsync(request);

        // Assert
        result.Success.Should().BeTrue();

        var inv = await db.InventoryItems.FirstOrDefaultAsync(i => i.MenuItemId == countableItem.Id);
        inv.Should().NotBeNull();
        inv!.ReservedStock.Should().Be(5);
        inv.AvailableStock.Should().Be(45); // 50 - 5 = 45

        var tx = await db.InventoryTransactions.FirstOrDefaultAsync(t => t.Type == InventoryTransactionType.Reservation);
        tx.Should().NotBeNull();
        tx!.Quantity.Should().Be(5);
    }

    [Fact]
    public async Task RequestBillAsync_WithValidToken_ShouldTransitionTableToBillingRequested()
    {
        // Arrange
        var (db, service, table, _, _) = CreateTestFixture(nameof(RequestBillAsync_WithValidToken_ShouldTransitionTableToBillingRequested));
        var token = _tokenService.GenerateToken(table.Id);
        table.Occupy();
        await db.SaveChangesAsync();

        // Act
        var result = await service.RequestBillAsync(table.Id, token);

        // Assert
        result.Should().BeTrue();
        var updatedTable = await db.RestaurantTables.FindAsync(table.Id);
        updatedTable!.Status.Should().Be(TableStatus.BillingRequested);

        var outbox = await db.OutboxMessages.FirstOrDefaultAsync(m => m.EventType == "TableStatusChanged" && m.PayloadJson.Contains("BillingRequested"));
        outbox.Should().NotBeNull();
    }
}
