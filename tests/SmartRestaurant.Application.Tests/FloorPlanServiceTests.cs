namespace SmartRestaurant.Application.Tests;

using FluentAssertions;
using SmartRestaurant.Application.DTOs;
using SmartRestaurant.Application.Services;
using SmartRestaurant.Domain.Entities;
using SmartRestaurant.Domain.Enums;
using SmartRestaurant.Domain.Exceptions;
using Xunit;

public class FloorPlanServiceTests
{
    private readonly Guid _branchId = Guid.NewGuid();

    [Fact]
    public async Task CreateDiningArea_ShouldPersistAndReturnDto()
    {
        // Arrange
        using var context = TestDbContextFactory.Create(nameof(CreateDiningArea_ShouldPersistAndReturnDto));
        var service = new FloorPlanService(context);

        var request = new CreateDiningAreaRequest(_branchId, "Tầng Thượng", AreaType.Outdoor, 1);

        // Act
        var result = await service.CreateDiningAreaAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Tầng Thượng");
        result.Type.Should().Be(AreaType.Outdoor);
    }

    [Fact]
    public async Task CreateTable_WithUniqueCode_ShouldSucceed()
    {
        // Arrange
        using var context = TestDbContextFactory.Create(nameof(CreateTable_WithUniqueCode_ShouldSucceed));
        var area = new DiningArea(_branchId, "Phòng Lạnh", AreaType.Indoor, 1);
        context.DiningAreas.Add(area);
        await context.SaveChangesAsync();

        var service = new FloorPlanService(context);
        var request = new CreateTableRequest(_branchId, area.Id, null, "PL-10", "Bàn Lạnh 10", 4, TableShape.Rectangle);

        // Act
        var result = await service.CreateTableAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Code.Should().Be("PL-10");
        result.Status.Should().Be(TableStatus.Available);
    }

    [Fact]
    public async Task CreateTable_WithDuplicateCode_ShouldThrowDomainValidationException()
    {
        // Arrange
        using var context = TestDbContextFactory.Create(nameof(CreateTable_WithDuplicateCode_ShouldThrowDomainValidationException));
        var area = new DiningArea(_branchId, "Phòng Lạnh", AreaType.Indoor, 1);
        context.DiningAreas.Add(area);
        var existingTable = new RestaurantTable(_branchId, area.Id, "PL-01", "Bàn 01", 4);
        context.RestaurantTables.Add(existingTable);
        await context.SaveChangesAsync();

        var service = new FloorPlanService(context);
        var request = new CreateTableRequest(_branchId, area.Id, null, "PL-01", "Bàn trùng mã", 4, TableShape.Rectangle);

        // Act
        var act = async () => await service.CreateTableAsync(request);

        // Assert
        await act.Should().ThrowAsync<DomainValidationException>()
            .WithMessage("*đã tồn tại*");
    }

    [Fact]
    public async Task UpdateTablePosition_ShouldUpdateCoordinates()
    {
        // Arrange
        using var context = TestDbContextFactory.Create(nameof(UpdateTablePosition_ShouldUpdateCoordinates));
        var area = new DiningArea(_branchId, "Sân Vườn", AreaType.Outdoor, 1);
        context.DiningAreas.Add(area);
        var table = new RestaurantTable(_branchId, area.Id, "SV-01", "Bàn 01", 4);
        context.RestaurantTables.Add(table);
        await context.SaveChangesAsync();

        var service = new FloorPlanService(context);
        var updateRequest = new UpdateTablePositionRequest(table.Id, 250, 350, 110, 95, 15);

        // Act
        var success = await service.UpdateTablePositionAsync(updateRequest);

        // Assert
        success.Should().BeTrue();
        var updated = await service.GetTableByIdAsync(table.Id);
        updated!.PositionX.Should().Be(250);
        updated.PositionY.Should().Be(350);
        updated.Width.Should().Be(110);
        updated.Height.Should().Be(95);
        updated.Rotation.Should().Be(15);
    }

    [Fact]
    public async Task UpdateTableStatus_ShouldTransitionStatus()
    {
        // Arrange
        using var context = TestDbContextFactory.Create(nameof(UpdateTableStatus_ShouldTransitionStatus));
        var area = new DiningArea(_branchId, "Sân Vườn", AreaType.Outdoor, 1);
        context.DiningAreas.Add(area);
        var table = new RestaurantTable(_branchId, area.Id, "SV-02", "Bàn 02", 4);
        context.RestaurantTables.Add(table);
        await context.SaveChangesAsync();

        var service = new FloorPlanService(context);

        // Act: Occupy table
        var success = await service.UpdateTableStatusAsync(table.Id, TableStatus.Occupied);

        // Assert
        success.Should().BeTrue();
        var updated = await service.GetTableByIdAsync(table.Id);
        updated!.Status.Should().Be(TableStatus.Occupied);
    }
}
