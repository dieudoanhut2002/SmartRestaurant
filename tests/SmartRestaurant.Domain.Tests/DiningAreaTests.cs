namespace SmartRestaurant.Domain.Tests;

using FluentAssertions;
using SmartRestaurant.Domain.Entities;
using SmartRestaurant.Domain.Enums;
using SmartRestaurant.Domain.Exceptions;
using Xunit;

public class DiningAreaTests
{
    private readonly Guid _branchId = Guid.NewGuid();

    [Fact]
    public void CreateDiningArea_WithValidParameters_ShouldInitializeCorrectly()
    {
        // Act
        var area = new DiningArea(_branchId, "Tầng 1 - Sân vườn", AreaType.Outdoor, 1);

        // Assert
        area.BranchId.Should().Be(_branchId);
        area.Name.Should().Be("Tầng 1 - Sân vườn");
        area.Type.Should().Be(AreaType.Outdoor);
        area.DisplayOrder.Should().Be(1);
        area.IsActive.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateDiningArea_WithEmptyName_ShouldThrowDomainValidationException(string empty)
    {
        // Act
        var act = () => new DiningArea(_branchId, empty, AreaType.Indoor);

        // Assert
        act.Should().Throw<DomainValidationException>()
            .WithMessage("*Tên khu vực*");
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveToFalse()
    {
        // Arrange
        var area = new DiningArea(_branchId, "Phòng VIP", AreaType.VIP);

        // Act
        area.Deactivate();

        // Assert
        area.IsActive.Should().BeFalse();
        area.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Activate_ShouldSetIsActiveToTrue()
    {
        // Arrange
        var area = new DiningArea(_branchId, "Phòng VIP", AreaType.VIP);
        area.Deactivate();

        // Act
        area.Activate();

        // Assert
        area.IsActive.Should().BeTrue();
    }
}
