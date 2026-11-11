namespace SmartRestaurant.Domain.Tests;

using FluentAssertions;
using SmartRestaurant.Domain.Entities;
using SmartRestaurant.Domain.Enums;
using SmartRestaurant.Domain.Exceptions;
using Xunit;

public class RestaurantTableTests
{
    private readonly Guid _branchId = Guid.NewGuid();
    private readonly Guid _areaId = Guid.NewGuid();

    [Fact]
    public void CreateTable_WithValidParameters_ShouldInitializeCorrectly()
    {
        // Act
        var table = new RestaurantTable(_branchId, _areaId, "T01", "Bàn 01", 4, shape: TableShape.Rectangle, posX: 100, posY: 200, width: 90, height: 90);

        // Assert
        table.BranchId.Should().Be(_branchId);
        table.DiningAreaId.Should().Be(_areaId);
        table.Code.Should().Be("T01");
        table.Name.Should().Be("Bàn 01");
        table.Capacity.Should().Be(4);
        table.Status.Should().Be(TableStatus.Available);
        table.Shape.Should().Be(TableShape.Rectangle);
        table.PositionX.Should().Be(100);
        table.PositionY.Should().Be(200);
        table.Width.Should().Be(90);
        table.Height.Should().Be(90);
        table.IsActive.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-5)]
    public void CreateTable_WithInvalidCapacity_ShouldThrowDomainValidationException(int capacity)
    {
        // Act
        var act = () => new RestaurantTable(_branchId, _areaId, "T01", "Bàn 01", capacity);

        // Assert
        act.Should().Throw<DomainValidationException>()
            .WithMessage("*Sức chứa*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateTable_WithEmptyCodeOrName_ShouldThrowDomainValidationException(string empty)
    {
        // Act & Assert
        var actCode = () => new RestaurantTable(_branchId, _areaId, empty, "Bàn 01", 4);
        actCode.Should().Throw<DomainValidationException>();

        var actName = () => new RestaurantTable(_branchId, _areaId, "T01", empty, 4);
        actName.Should().Throw<DomainValidationException>();
    }

    [Fact]
    public void Occupy_WhenTableIsAvailable_ShouldTransitionToOccupied()
    {
        // Arrange
        var table = new RestaurantTable(_branchId, _areaId, "T01", "Bàn 01", 4);

        // Act
        table.Occupy();

        // Assert
        table.Status.Should().Be(TableStatus.Occupied);
    }

    [Fact]
    public void Occupy_WhenTableIsReserved_ShouldTransitionToOccupied()
    {
        // Arrange
        var table = new RestaurantTable(_branchId, _areaId, "T01", "Bàn 01", 4);
        table.Reserve();

        // Act
        table.Occupy();

        // Assert
        table.Status.Should().Be(TableStatus.Occupied);
    }

    [Fact]
    public void Occupy_WhenTableIsAlreadyOccupied_ShouldThrowInvalidTableStateTransitionException()
    {
        // Arrange
        var table = new RestaurantTable(_branchId, _areaId, "T01", "Bàn 01", 4);
        table.Occupy();

        // Act
        var act = () => table.Occupy();

        // Assert
        act.Should().Throw<InvalidTableStateTransitionException>()
            .Where(e => e.CurrentStatus == TableStatus.Occupied && e.TargetStatus == TableStatus.Occupied);
    }

    [Fact]
    public void ReleaseToCleaning_WhenTableIsOccupied_ShouldTransitionToCleaningNeeded()
    {
        // Arrange
        var table = new RestaurantTable(_branchId, _areaId, "T01", "Bàn 01", 4);
        table.Occupy();

        // Act
        table.ReleaseToCleaning();

        // Assert
        table.Status.Should().Be(TableStatus.CleaningNeeded);
    }

    [Fact]
    public void ReleaseToCleaning_WhenTableIsAvailable_ShouldThrowInvalidTableStateTransitionException()
    {
        // Arrange
        var table = new RestaurantTable(_branchId, _areaId, "T01", "Bàn 01", 4);

        // Act
        var act = () => table.ReleaseToCleaning();

        // Assert
        act.Should().Throw<InvalidTableStateTransitionException>()
            .Where(e => e.CurrentStatus == TableStatus.Available && e.TargetStatus == TableStatus.CleaningNeeded);
    }

    [Fact]
    public void CompleteCleaning_WhenTableIsCleaningNeeded_ShouldTransitionToAvailable()
    {
        // Arrange
        var table = new RestaurantTable(_branchId, _areaId, "T01", "Bàn 01", 4);
        table.Occupy();
        table.ReleaseToCleaning();

        // Act
        table.CompleteCleaning();

        // Assert
        table.Status.Should().Be(TableStatus.Available);
    }

    [Fact]
    public void CompleteCleaning_WhenTableIsNotCleaningNeeded_ShouldThrowInvalidTableStateTransitionException()
    {
        // Arrange
        var table = new RestaurantTable(_branchId, _areaId, "T01", "Bàn 01", 4);

        // Act
        var act = () => table.CompleteCleaning();

        // Assert
        act.Should().Throw<InvalidTableStateTransitionException>();
    }

    [Fact]
    public void Reserve_WhenTableIsAvailable_ShouldTransitionToReserved()
    {
        // Arrange
        var table = new RestaurantTable(_branchId, _areaId, "T01", "Bàn 01", 4);

        // Act
        table.Reserve();

        // Assert
        table.Status.Should().Be(TableStatus.Reserved);
    }

    [Fact]
    public void CancelReservation_WhenTableIsReserved_ShouldTransitionToAvailable()
    {
        // Arrange
        var table = new RestaurantTable(_branchId, _areaId, "T01", "Bàn 01", 4);
        table.Reserve();

        // Act
        table.CancelReservation();

        // Assert
        table.Status.Should().Be(TableStatus.Available);
    }

    [Fact]
    public void UpdateLayout_ShouldUpdateCoordinatesCorrectly()
    {
        // Arrange
        var table = new RestaurantTable(_branchId, _areaId, "T01", "Bàn 01", 4);

        // Act
        table.UpdateLayout(150.5, 250.7, 120, 100, 45);

        // Assert
        table.PositionX.Should().Be(150.5);
        table.PositionY.Should().Be(250.7);
        table.Width.Should().Be(120);
        table.Height.Should().Be(100);
        table.Rotation.Should().Be(45);
    }
}
