namespace SmartRestaurant.Application.Tests;

using FluentAssertions;
using SmartRestaurant.Application.DTOs;
using SmartRestaurant.Application.Services;
using SmartRestaurant.Domain.Entities;
using SmartRestaurant.Domain.Enums;
using SmartRestaurant.Domain.Exceptions;
using Xunit;

public class MenuServiceTests
{
    private readonly Guid _branchId = Guid.NewGuid();

    [Fact]
    public async Task CreateMenuItem_WithValidData_ShouldSucceed()
    {
        // Arrange
        using var context = TestDbContextFactory.Create(nameof(CreateMenuItem_WithValidData_ShouldSucceed));
        var category = new MenuCategory(_branchId, "Món Chính", "Món chính");
        context.MenuCategories.Add(category);
        await context.SaveChangesAsync();

        var service = new MenuService(context);
        var request = new CreateMenuItemRequest(
            _branchId,
            category.Id,
            "MC-99",
            "Bò Xào Nấm",
            160000m,
            "Thịt bò tươi xào nấm",
            "/images/bo-xao.jpg",
            PrepStation.HotKitchen,
            DirectFOHPick: false,
            IsCountable: true,
            InStockQuantity: 50);

        // Act
        var result = await service.CreateMenuItemAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Code.Should().Be("MC-99");
        result.BasePrice.Should().Be(160000m);
        result.IsAvailable.Should().BeTrue();
    }

    [Fact]
    public async Task ToggleAvailability_ShouldFlip86Status()
    {
        // Arrange
        using var context = TestDbContextFactory.Create(nameof(ToggleAvailability_ShouldFlip86Status));
        var category = new MenuCategory(_branchId, "Món Chính", "Món chính");
        context.MenuCategories.Add(category);
        var item = new MenuItem(_branchId, category.Id, "MC-01", "Gà Hấp", 180000m);
        context.MenuItems.Add(item);
        await context.SaveChangesAsync();

        var service = new MenuService(context);

        // Act: Turn off availability (86)
        var success = await service.ToggleAvailabilityAsync(item.Id);

        // Assert
        success.Should().BeTrue();
        var updated = await service.GetMenuItemByIdAsync(item.Id);
        updated!.IsAvailable.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateStock_WhenZero_ShouldAutoDisableAvailabilityForCountableItem()
    {
        // Arrange
        using var context = TestDbContextFactory.Create(nameof(UpdateStock_WhenZero_ShouldAutoDisableAvailabilityForCountableItem));
        var category = new MenuCategory(_branchId, "Đồ Uống", "Đồ uống");
        context.MenuCategories.Add(category);
        var item = new MenuItem(_branchId, category.Id, "DU-01", "Bia Heineken", 30000m, isCountable: true, inStockQuantity: 10);
        context.MenuItems.Add(item);
        await context.SaveChangesAsync();

        var service = new MenuService(context);

        // Act: Set stock to 0
        var success = await service.UpdateStockAsync(item.Id, 0);

        // Assert
        success.Should().BeTrue();
        var updated = await service.GetMenuItemByIdAsync(item.Id);
        updated!.InStockQuantity.Should().Be(0);
        updated.IsAvailable.Should().BeFalse(); // Auto 86 when countable item is out of stock per BR-INV-004
    }
}
