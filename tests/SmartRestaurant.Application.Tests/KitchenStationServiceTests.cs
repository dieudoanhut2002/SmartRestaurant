namespace SmartRestaurant.Application.Tests;

using FluentAssertions;
using SmartRestaurant.Application.DTOs;
using SmartRestaurant.Application.Services;
using SmartRestaurant.Domain.Entities;
using SmartRestaurant.Domain.Enums;
using SmartRestaurant.Domain.Exceptions;
using Xunit;

public class KitchenStationServiceTests
{
    private readonly Guid _branchId = Guid.NewGuid();

    [Fact]
    public async Task CreateStation_WithValidData_ShouldSucceed()
    {
        // Arrange
        using var context = TestDbContextFactory.Create(nameof(CreateStation_WithValidData_ShouldSucceed));
        var service = new KitchenStationService(context);
        var request = new CreateKitchenStationRequest(
            _branchId,
            "KS-HOT",
            "Bếp Nóng",
            PrepStation.HotKitchen,
            "Trạm chế biến món nóng",
            1,
            "192.168.1.201");

        // Act
        var station = await service.CreateStationAsync(request);

        // Assert
        station.Should().NotBeNull();
        station.Code.Should().Be("KS-HOT");
        station.StationTypeName.Should().Be("Bếp Nóng");
        station.PrinterIpAddress.Should().Be("192.168.1.201");
    }

    [Fact]
    public async Task CreateStation_DuplicateCode_ShouldThrowDomainValidationException()
    {
        // Arrange
        using var context = TestDbContextFactory.Create(nameof(CreateStation_DuplicateCode_ShouldThrowDomainValidationException));
        var service = new KitchenStationService(context);
        var request = new CreateKitchenStationRequest(
            _branchId,
            "KS-HOT",
            "Bếp Nóng",
            PrepStation.HotKitchen,
            "Trạm chế biến món nóng",
            1,
            "192.168.1.201");

        await service.CreateStationAsync(request);

        // Act & Assert
        var act = async () => await service.CreateStationAsync(request);
        await act.Should().ThrowAsync<DomainValidationException>()
            .WithMessage("*đã tồn tại*");
    }

    [Fact]
    public async Task AssignDishToStation_ShouldUpdateDishKitchenStationId()
    {
        // Arrange
        using var context = TestDbContextFactory.Create(nameof(AssignDishToStation_ShouldUpdateDishKitchenStationId));
        var cat = new MenuCategory(_branchId, "Món Chính");
        context.MenuCategories.Add(cat);
        var dish = new MenuItem(_branchId, cat.Id, "MC-01", "Bò Kho", 90000m);
        context.MenuItems.Add(dish);
        var station = new KitchenStation(_branchId, "KS-HOT", "Bếp Nóng", PrepStation.HotKitchen);
        context.KitchenStations.Add(station);
        await context.SaveChangesAsync();

        var service = new KitchenStationService(context);

        // Act
        var result = await service.AssignDishToStationAsync(new AssignDishToStationRequest(dish.Id, station.Id, PrepStation.HotKitchen));

        // Assert
        result.Should().BeTrue();
        var updatedStation = await service.GetStationByIdAsync(station.Id);
        updatedStation.Should().NotBeNull();
        updatedStation!.AssignedDishCount.Should().Be(1);
        updatedStation.AssignedDishes.Should().Contain(d => d.Id == dish.Id);
    }
}
