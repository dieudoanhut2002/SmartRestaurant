namespace SmartRestaurant.Application.Tests;

using FluentAssertions;
using SmartRestaurant.Application.DTOs;
using SmartRestaurant.Application.Services;
using SmartRestaurant.Domain.Entities;
using SmartRestaurant.Domain.Enums;
using SmartRestaurant.Domain.Exceptions;
using Xunit;

public class InventoryServiceTests
{
    private readonly Guid _branchId = Guid.NewGuid();

    [Fact]
    public async Task Inventory_AvailableStock_ShouldFollowInvariantINV03()
    {
        // Arrange
        using var context = TestDbContextFactory.Create(nameof(Inventory_AvailableStock_ShouldFollowInvariantINV03));
        var item = new InventoryItem(_branchId, "INV-01", "Bia Heineken", "Đồ Uống", "Lon", initialPhysicalStock: 100, minimumThreshold: 20);
        item.ReserveStock(15);
        context.InventoryItems.Add(item);
        await context.SaveChangesAsync();

        var service = new InventoryService(context);

        // Act
        var result = await service.GetItemByIdAsync(item.Id);

        // Assert: Available = Physical - Reserved (100 - 15 = 85)
        result.Should().NotBeNull();
        result!.PhysicalStock.Should().Be(100);
        result.ReservedStock.Should().Be(15);
        result.AvailableStock.Should().Be(85);
        result.IsLowStock.Should().BeFalse();
    }

    [Fact]
    public async Task ReceiveStock_ShouldIncreasePhysicalStockAndRecordTransaction()
    {
        // Arrange
        using var context = TestDbContextFactory.Create(nameof(ReceiveStock_ShouldIncreasePhysicalStockAndRecordTransaction));
        var item = new InventoryItem(_branchId, "INV-02", "Bia Sài Gòn", "Đồ Uống", "Lon", initialPhysicalStock: 50, minimumThreshold: 10);
        context.InventoryItems.Add(item);
        await context.SaveChangesAsync();

        var service = new InventoryService(context);
        var request = new ReceiveStockRequest(_branchId, item.Id, 30, "Nhập kho đợt 2", "Thủ Kho");

        // Act
        var result = await service.ReceiveStockAsync(request);

        // Assert
        result.PhysicalStock.Should().Be(80);
        result.AvailableStock.Should().Be(80);

        var transactions = await service.GetTransactionsAsync(_branchId, item.Id);
        transactions.Should().ContainSingle(t => t.Type == InventoryTransactionType.Receipt && t.Quantity == 30);
    }

    [Fact]
    public async Task AdjustStock_LowerThanReserved_ShouldThrowDomainValidationException()
    {
        // Arrange
        using var context = TestDbContextFactory.Create(nameof(AdjustStock_LowerThanReserved_ShouldThrowDomainValidationException));
        var item = new InventoryItem(_branchId, "INV-03", "Lavie", "Nước", "Chai", initialPhysicalStock: 50, minimumThreshold: 10);
        item.ReserveStock(20);
        context.InventoryItems.Add(item);
        await context.SaveChangesAsync();

        var service = new InventoryService(context);
        var request = new AdjustStockRequest(_branchId, item.Id, 15, "Kiểm kê thiếu hụt", "Thủ Kho");

        // Act & Assert
        var act = async () => await service.AdjustStockAsync(request);
        await act.Should().ThrowAsync<DomainValidationException>()
            .WithMessage("*nhỏ hơn số lượng đang tạm giữ*");
    }

    [Fact]
    public async Task GetSummary_ShouldAggregateStockAcrossAllItems()
    {
        // Arrange
        using var context = TestDbContextFactory.Create(nameof(GetSummary_ShouldAggregateStockAcrossAllItems));
        var item1 = new InventoryItem(_branchId, "I1", "Mặt hàng 1", "Nhóm 1", "Cái", initialPhysicalStock: 100, minimumThreshold: 30);
        item1.ReserveStock(20);
        var item2 = new InventoryItem(_branchId, "I2", "Mặt hàng 2", "Nhóm 1", "Cái", initialPhysicalStock: 10, minimumThreshold: 20); // Low stock

        context.InventoryItems.AddRange(item1, item2);
        await context.SaveChangesAsync();

        var service = new InventoryService(context);

        // Act
        var summary = await service.GetSummaryAsync(_branchId);

        // Assert
        summary.TotalItems.Should().Be(2);
        summary.LowStockItems.Should().Be(1);
        summary.TotalPhysicalCount.Should().Be(110);
        summary.TotalReservedCount.Should().Be(20);
        summary.TotalAvailableCount.Should().Be(90);
    }
}
