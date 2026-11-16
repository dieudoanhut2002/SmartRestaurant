namespace SmartRestaurant.Domain.Tests;

using FluentAssertions;
using SmartRestaurant.Domain.Entities;
using SmartRestaurant.Domain.Enums;
using SmartRestaurant.Domain.ValueObjects;
using Xunit;

public class SystemLicenseTests
{
    private const string TestInstallationId = "INSTALL-SR-2026-AAAA-BBBB-CCCC";

    private static SystemLicense CreateTestLicense(
        DateTime? validFrom = null,
        DateTime? expiresAt = null,
        GracePolicy? gracePolicy = null,
        string installationId = TestInstallationId,
        List<string>? features = null)
    {
        var now = DateTime.UtcNow;
        return new SystemLicense(
            licenseId: "LIC-TEST-001",
            customerId: "Nhà Hàng Biển Đông",
            installationId: installationId,
            productCode: "SmartRestaurant",
            edition: "Standard",
            features: features ?? ["TableManagement", "Menu", "KitchenKDS"],
            issuedAt: now.AddDays(-30),
            validFrom: validFrom ?? now.AddDays(-30),
            expiresAt: expiresAt ?? now.AddDays(30),
            licenseVersion: 1,
            gracePolicy: gracePolicy ?? GracePolicy.Default,
            rawDocument: "raw-document",
            digitalSignature: "dummy-signature");
    }

    [Fact]
    public void EvaluateState_WhenActiveAndValid_ShouldReturnActive()
    {
        // Arrange
        var license = CreateTestLicense();

        // Act
        var state = license.EvaluateState(TestInstallationId, DateTime.UtcNow);

        // Assert
        state.Should().Be(LicenseState.Active);
    }

    [Fact]
    public void EvaluateState_WhenClockRollbackDetected_ShouldReturnVerificationRequired()
    {
        // Arrange
        var license = CreateTestLicense();

        // Act
        var state = license.EvaluateState(TestInstallationId, DateTime.UtcNow, clockRollbackDetected: true);

        // Assert
        state.Should().Be(LicenseState.VerificationRequired);
    }

    [Fact]
    public void EvaluateState_WhenInstallationIdMismatch_ShouldReturnInvalid()
    {
        // Arrange
        var license = CreateTestLicense(installationId: "DIFFERENT-INSTALLATION-ID");

        // Act
        var state = license.EvaluateState(TestInstallationId, DateTime.UtcNow);

        // Assert
        state.Should().Be(LicenseState.Invalid);
    }

    [Fact]
    public void EvaluateState_WhenNotYetValid_ShouldReturnInvalid()
    {
        // Arrange
        var license = CreateTestLicense(validFrom: DateTime.UtcNow.AddDays(5));

        // Act
        var state = license.EvaluateState(TestInstallationId, DateTime.UtcNow);

        // Assert
        state.Should().Be(LicenseState.Invalid);
    }

    [Fact]
    public void EvaluateState_WhenExpiringSoon_ShouldReturnExpiringSoon()
    {
        // Arrange (expires in 5 days, expiring soon threshold is 14 days)
        var license = CreateTestLicense(expiresAt: DateTime.UtcNow.AddDays(5));

        // Act
        var state = license.EvaluateState(TestInstallationId, DateTime.UtcNow);

        // Assert
        state.Should().Be(LicenseState.ExpiringSoon);
    }

    [Fact]
    public void EvaluateState_WhenExpiredWithinGracePeriod_ShouldReturnExpiredGrace()
    {
        // Arrange (expired 3 days ago, grace is 7 days)
        var license = CreateTestLicense(expiresAt: DateTime.UtcNow.AddDays(-3));

        // Act
        var state = license.EvaluateState(TestInstallationId, DateTime.UtcNow);

        // Assert
        state.Should().Be(LicenseState.ExpiredGrace);
    }

    [Fact]
    public void EvaluateState_WhenExpiredBeyondGracePeriod_ShouldReturnExpired()
    {
        // Arrange (expired 10 days ago, grace is 7 days)
        var license = CreateTestLicense(expiresAt: DateTime.UtcNow.AddDays(-10));

        // Act
        var state = license.EvaluateState(TestInstallationId, DateTime.UtcNow);

        // Assert
        state.Should().Be(LicenseState.Expired);
    }

    [Fact]
    public void EvaluateState_WhenOfflineAllowanceExceeded_ShouldReturnOfflineGrace()
    {
        // Arrange (last validated 35 days ago, allowance is 30 days, expires in 200 days)
        var license = CreateTestLicense(expiresAt: DateTime.UtcNow.AddDays(200));
        var lastValidated = DateTime.UtcNow.AddDays(-35);

        // Act
        var state = license.EvaluateState(TestInstallationId, DateTime.UtcNow, lastValidatedUtc: lastValidated);

        // Assert
        state.Should().Be(LicenseState.OfflineGrace);
    }

    [Fact]
    public void EvaluateState_WhenDeactivated_ShouldReturnUninitialized()
    {
        // Arrange
        var license = CreateTestLicense();
        license.Deactivate();

        // Act
        var state = license.EvaluateState(TestInstallationId, DateTime.UtcNow);

        // Assert
        state.Should().Be(LicenseState.Uninitialized);
    }

    [Theory]
    [InlineData("TableManagement", true)]
    [InlineData("Menu", true)]
    [InlineData("KitchenKDS", true)]
    [InlineData("VietQR", false)]
    public void IsFeatureEnabled_ShouldMatchLicensedFeatures(string featureCode, bool expected)
    {
        // Arrange
        var license = CreateTestLicense(features: ["TableManagement", "Menu", "KitchenKDS"]);

        // Act
        var result = license.IsFeatureEnabled(featureCode);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void IsFeatureEnabled_WithWildcard_ShouldEnableAllFeatures()
    {
        // Arrange
        var license = CreateTestLicense(features: ["*"]);

        // Act & Assert
        license.IsFeatureEnabled("AnyArbitraryModule").Should().BeTrue();
    }
}
