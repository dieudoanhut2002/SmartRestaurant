namespace SmartRestaurant.Application.Tests;

using FluentAssertions;
using SmartRestaurant.Application.DTOs;
using SmartRestaurant.Application.Services;
using SmartRestaurant.Domain.Entities;
using Xunit;

public class SettingServiceTests
{
    [Fact]
    public async Task UpdateCutoff_ShouldUpdateBranchCutoffTime()
    {
        // Arrange
        using var context = TestDbContextFactory.Create(nameof(UpdateCutoff_ShouldUpdateBranchCutoffTime));
        var branch = new Branch("Nhà Hàng A", "123 Đường B", "0900000000", new TimeSpan(6, 0, 0));
        context.Branches.Add(branch);
        await context.SaveChangesAsync();

        var service = new SettingService(context);
        var newCutoff = new TimeSpan(5, 30, 0);

        // Act
        var result = await service.UpdateCutoffAsync(new UpdateBusinessDayCutoffRequest(branch.Id, newCutoff));

        // Assert
        result.BusinessDayCutoffTime.Should().Be(newCutoff);
        result.FormattedCutoffTime.Should().Be("05:30");
    }

    [Fact]
    public async Task UpdateVipSurcharges_ShouldUpdateRateAndFixedAmount()
    {
        // Arrange
        using var context = TestDbContextFactory.Create(nameof(UpdateVipSurcharges_ShouldUpdateRateAndFixedAmount));
        var branch = new Branch("Nhà Hàng VIP", "Khu Đô Thị", "0911111111");
        context.Branches.Add(branch);
        await context.SaveChangesAsync();

        var service = new SettingService(context);

        // Act
        var result = await service.UpdateVipSurchargesAsync(new UpdateVipSurchargeRequest(branch.Id, 15m, 250000m));

        // Assert
        result.VipRoomSurchargeRate.Should().Be(15m);
        result.VipRoomSurchargeFixedAmount.Should().Be(250000m);
    }

    [Theory]
    [InlineData("Cafe", false, true, true, true, 0, 0)]
    [InlineData("BeerPub", false, true, true, true, 5, 100000)]
    [InlineData("Alacarte", true, true, true, true, 10, 150000)]
    public async Task ApplyPreset_ShouldConfigureModulesAndSurchargesAccordingly(
        string presetName,
        bool expectReservation,
        bool expectVietQr,
        bool expectInventory,
        bool expectReporting,
        decimal expectRate,
        decimal expectFixed)
    {
        // Arrange
        using var context = TestDbContextFactory.Create(nameof(ApplyPreset_ShouldConfigureModulesAndSurchargesAccordingly) + presetName);
        var branch = new Branch("Nhà Hàng Preset Test", "Địa chỉ", "0900000000");
        context.Branches.Add(branch);
        await context.SaveChangesAsync();

        var service = new SettingService(context);

        // Act
        var result = await service.ApplyPresetAsync(new ApplyPresetRequest(branch.Id, presetName));

        // Assert
        result.IsTableReservationEnabled.Should().Be(expectReservation);
        result.IsVietQrEnabled.Should().Be(expectVietQr);
        result.IsInventoryEnabled.Should().Be(expectInventory);
        result.IsReportingEnabled.Should().Be(expectReporting);
        result.VipRoomSurchargeRate.Should().Be(expectRate);
        result.VipRoomSurchargeFixedAmount.Should().Be(expectFixed);
    }
}
