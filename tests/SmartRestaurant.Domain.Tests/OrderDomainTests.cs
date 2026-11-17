namespace SmartRestaurant.Domain.Tests;

using FluentAssertions;
using SmartRestaurant.Domain.Entities;
using SmartRestaurant.Domain.Enums;
using SmartRestaurant.Domain.Exceptions;
using Xunit;

public class OrderDomainTests
{
    private readonly Guid _branchId = Guid.NewGuid();
    private readonly Guid _diningSessionId = Guid.NewGuid();
    private readonly Guid _staffId = Guid.NewGuid();

    [Fact]
    public void Order_WhenCreated_ShouldInitializeWithCorrectDefaults()
    {
        // Arrange
        var businessDate = new DateTime(2026, 11, 16, 6, 0, 0, DateTimeKind.Utc);

        // Act
        var order = new Order(_branchId, _diningSessionId, "ORD-001", businessDate);

        // Assert
        order.BranchId.Should().Be(_branchId);
        order.DiningSessionId.Should().Be(_diningSessionId);
        order.OrderCode.Should().Be("ORD-001");
        order.BusinessDate.Should().Be(businessDate.Date);
        order.Status.Should().Be(OrderStatus.Active);
        order.IsSettled.Should().BeFalse();
        order.Subtotal.Should().Be(0);
        order.TotalAmount.Should().Be(0);
        order.Rounds.Should().BeEmpty();
    }

    [Fact]
    public void Order_AddRound_ShouldCalculateTotalsCorrectly()
    {
        // Arrange
        var order = new Order(_branchId, _diningSessionId, "ORD-001", DateTime.UtcNow);
        var round = new OrderRound(order.Id, 1, _staffId, "IDEM-001");

        var item1 = new OrderItem(round.Id, Guid.NewGuid(), "Bia Tiger Lon", 25000m, 2); // 50,000
        var item2 = new OrderItem(round.Id, Guid.NewGuid(), "Bò né", 100000m, 1);
        item2.AddModifier(new OrderItemModifier(item2.Id, Guid.NewGuid(), "Thêm trứng", 10000m)); // 110,000

        round.AddItem(item1);
        round.AddItem(item2);

        // Act
        order.AddRound(round);
        order.RecalculateTotals(surchargePercent: 10, surchargeFixed: 20000, discount: 10000);

        // Assert
        // Subtotal = 50,000 + 110,000 = 160,000
        // Surcharges = 20,000 + (160,000 * 10%) = 36,000
        // Discount = 10,000
        // TotalAmount = 160,000 + 36,000 - 10,000 = 186,000
        order.Subtotal.Should().Be(160000m);
        order.Surcharges.Should().Be(36000m);
        order.DiscountAmount.Should().Be(10000m);
        order.TotalAmount.Should().Be(186000m);
    }

    [Fact]
    public void Order_WhenSettled_ShouldPreventModificationsPerOD13()
    {
        // Arrange
        var order = new Order(_branchId, _diningSessionId, "ORD-001", DateTime.UtcNow);
        order.Settle();

        // Act & Assert
        order.IsSettled.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Settled);

        var round = new OrderRound(order.Id, 1, _staffId, "IDEM-002");
        var actAdd = () => order.AddRound(round);
        actAdd.Should().Throw<DomainValidationException>()
            .WithMessage("*OD-13*");

        var actRecalc = () => order.RecalculateTotals();
        actRecalc.Should().Throw<DomainValidationException>();
    }

    [Fact]
    public void OrderItem_DirectFOHPick_ShouldStartWithReadyToServeStatus()
    {
        // Arrange & Act
        var roundId = Guid.NewGuid();
        var fohItem = new OrderItem(roundId, Guid.NewGuid(), "Nước ngọt lon", 15000, 1, directFOHPick: true);
        var kitchenItem = new OrderItem(roundId, Guid.NewGuid(), "Gà rán", 80000, 1, directFOHPick: false);

        // Assert
        fohItem.DirectFOHPick.Should().BeTrue();
        fohItem.Status.Should().Be(OrderItemStatus.ReadyToServe);

        kitchenItem.DirectFOHPick.Should().BeFalse();
        kitchenItem.Status.Should().Be(OrderItemStatus.Pending);
    }

    [Fact]
    public void KitchenTicket_Lifecycle_ShouldTransitionCorrectly()
    {
        // Arrange
        var ticket = new KitchenTicket(Guid.NewGuid(), Guid.NewGuid(), "TICK-001", "[{\"name\":\"Lẩu Thái\"}]");

        // Assert initial
        ticket.Status.Should().Be(KitchenTicketStatus.Pending);

        // Act & Assert in-prep
        ticket.StartPreparation();
        ticket.Status.Should().Be(KitchenTicketStatus.InPreparation);

        // Act & Assert completed
        ticket.Complete();
        ticket.Status.Should().Be(KitchenTicketStatus.Completed);
        ticket.CompletedAt.Should().NotBeNull();
    }

    [Fact]
    public void DiningSession_TransferToTable_ShouldUpdateTableIdWhenActive()
    {
        // Arrange
        var table1 = Guid.NewGuid();
        var table2 = Guid.NewGuid();
        var session = new DiningSession(_branchId, table1, "SES-001", 3);

        // Act
        session.TransferToTable(table2);

        // Assert
        session.TableId.Should().Be(table2);
    }
}
