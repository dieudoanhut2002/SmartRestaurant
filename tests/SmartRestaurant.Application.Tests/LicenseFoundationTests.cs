namespace SmartRestaurant.Application.Tests;

using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using SmartRestaurant.Application.DTOs;
using SmartRestaurant.Domain.Enums;
using SmartRestaurant.Infrastructure.Licensing;
using Xunit;

public class LicenseFoundationTests : IDisposable
{
    private readonly string _testDirectory;
    private readonly LicenseOptions _options;
    private readonly LicenseSignatureValidator _validator;
    private readonly InstallationIdentityService _identityService;
    private readonly LicenseStorageService _storageService;
    private readonly LicenseClockService _clockService;

    public LicenseFoundationTests()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), "SR_LicTest_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_testDirectory);

        _options = new LicenseOptions
        {
            Mode = "Production",
            ProductCode = "SmartRestaurant",
            IssuerPublicKeyPem = TestLicenseSigner.TestIssuerPublicKeyPem,
            DefaultOfflineAllowanceDays = 30,
            DefaultExpiryGraceDays = 7,
            ExpiringSoonDays = 14,
            ClockDriftToleranceMinutes = 10,
            StorageDirectory = _testDirectory
        };

        var optionsWrapper = Options.Create(_options);
        _validator = new LicenseSignatureValidator(optionsWrapper, NullLogger<LicenseSignatureValidator>.Instance);
        _identityService = new InstallationIdentityService(optionsWrapper, NullLogger<InstallationIdentityService>.Instance);
        _storageService = new LicenseStorageService(optionsWrapper, NullLogger<LicenseStorageService>.Instance);
        _clockService = new LicenseClockService();
    }

    public void Dispose()
    {
        try
        {
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }
        catch
        {
            // Ignore cleanup errors
        }
    }

    private LicenseService CreateService(LicenseOptions? customOptions = null)
    {
        var opts = Options.Create(customOptions ?? _options);
        return new LicenseService(
            _validator,
            _identityService,
            _storageService,
            _clockService,
            opts,
            NullLogger<LicenseService>.Instance);
    }

    // 1. Valid License signature
    [Fact]
    public void VerifySignature_WithValidSignature_ShouldReturnTrue()
    {
        // Arrange
        var model = new LicenseModel { CustomerId = "Nhà Hàng Biển Đông", ProductCode = "SmartRestaurant" };
        var doc = TestLicenseSigner.CreateSignedLicenseDocument(model);
        var parsed = LicenseStorageService.ParseLicenseEnvelope(doc);

        // Act
        var isValid = _validator.VerifySignature(parsed!.PayloadJson, parsed.SignatureBase64);

        // Assert
        isValid.Should().BeTrue();
    }

    // 2. Invalid License signature
    [Fact]
    public void VerifySignature_WithInvalidSignature_ShouldReturnFalse()
    {
        // Arrange
        var model = new LicenseModel { CustomerId = "Nhà Hàng Biển Đông", ProductCode = "SmartRestaurant" };
        var doc = TestLicenseSigner.CreateSignedLicenseDocument(model);
        var parsed = LicenseStorageService.ParseLicenseEnvelope(doc);

        var corruptedSignature = Convert.ToBase64String(new byte[64]); // Random dummy bytes

        // Act
        var isValid = _validator.VerifySignature(parsed!.PayloadJson, corruptedSignature);

        // Assert
        isValid.Should().BeFalse();
    }

    // 3. Tampered License payload
    [Fact]
    public void VerifySignature_WithTamperedPayload_ShouldReturnFalse()
    {
        // Arrange
        var model = new LicenseModel { CustomerId = "Nhà Hàng Biển Đông", Edition = "Standard" };
        var doc = TestLicenseSigner.CreateSignedLicenseDocument(model);
        var parsed = LicenseStorageService.ParseLicenseEnvelope(doc);

        // Can thiệp sửa payload (thay đổi Standard thành Enterprise)
        var tamperedPayloadJson = parsed!.PayloadJson.Replace("Standard", "Enterprise");

        // Act
        var isValid = _validator.VerifySignature(tamperedPayloadJson, parsed.SignatureBase64);

        // Assert
        isValid.Should().BeFalse();
    }

    // 4. Expired License
    [Fact]
    public async Task ValidateAsync_WhenLicenseExpired_ShouldReturnExpired()
    {
        // Arrange
        var service = CreateService();
        var identity = await _identityService.GetOrCreateInstallationIdentityAsync();

        var model = new LicenseModel
        {
            CustomerId = "Nhà Hàng Biển Đông",
            InstallationId = identity.InstallationId,
            ProductCode = "SmartRestaurant",
            IssuedAt = DateTime.UtcNow.AddDays(-400),
            ValidFrom = DateTime.UtcNow.AddDays(-400),
            ExpiresAt = DateTime.UtcNow.AddDays(-20), // Quá hạn 20 ngày (vượt quá cả 7 ngày ân hạn)
            GracePolicy = new GracePolicyDto { ExpiryGraceDays = 7 }
        };

        var doc = TestLicenseSigner.CreateSignedLicenseDocument(model);
        var parsed = LicenseStorageService.ParseLicenseEnvelope(doc);
        await _storageService.SaveLicenseAsync(parsed!);

        // Act
        var result = await service.ValidateAsync();

        // Assert
        result.State.Should().Be(LicenseState.Expired);
        result.IsValid.Should().BeFalse();
        result.CanOperate.Should().BeFalse();
    }

    // 5. License not yet valid
    [Fact]
    public async Task ValidateAsync_WhenLicenseNotYetValid_ShouldReturnInvalid()
    {
        // Arrange
        var service = CreateService();
        var identity = await _identityService.GetOrCreateInstallationIdentityAsync();

        var model = new LicenseModel
        {
            CustomerId = "Nhà Hàng Biển Đông",
            InstallationId = identity.InstallationId,
            ProductCode = "SmartRestaurant",
            IssuedAt = DateTime.UtcNow,
            ValidFrom = DateTime.UtcNow.AddDays(7), // 7 ngày nữa mới bắt đầu có hiệu lực
            ExpiresAt = DateTime.UtcNow.AddYears(1)
        };

        var doc = TestLicenseSigner.CreateSignedLicenseDocument(model);
        var parsed = LicenseStorageService.ParseLicenseEnvelope(doc);
        await _storageService.SaveLicenseAsync(parsed!);

        // Act
        var result = await service.ValidateAsync();

        // Assert
        result.State.Should().Be(LicenseState.Invalid);
        result.Message.Should().ContainEquivalentOf("chưa");
    }

    // 6. InstallationId mismatch
    [Fact]
    public async Task ValidateAsync_WhenInstallationIdMismatch_ShouldReturnInvalid()
    {
        // Arrange
        var service = CreateService();

        var model = new LicenseModel
        {
            CustomerId = "Nhà Hàng Biển Đông",
            InstallationId = "WRONG-INSTALLATION-ID-9999",
            ProductCode = "SmartRestaurant",
            ValidFrom = DateTime.UtcNow.AddDays(-10),
            ExpiresAt = DateTime.UtcNow.AddYears(1)
        };

        var doc = TestLicenseSigner.CreateSignedLicenseDocument(model);
        var parsed = LicenseStorageService.ParseLicenseEnvelope(doc);
        await _storageService.SaveLicenseAsync(parsed!);

        // Act
        var result = await service.ValidateAsync();

        // Assert
        result.State.Should().Be(LicenseState.Invalid);
        result.Message.Should().Contain("InstallationId");
    }

    // 7. Unsupported License version
    [Fact]
    public async Task ValidateAsync_WhenUnsupportedLicenseVersion_ShouldReturnInvalid()
    {
        // Arrange
        var service = CreateService();
        var identity = await _identityService.GetOrCreateInstallationIdentityAsync();

        var model = new LicenseModel
        {
            CustomerId = "Nhà Hàng Biển Đông",
            InstallationId = identity.InstallationId,
            ProductCode = "SmartRestaurant",
            LicenseVersion = 99, // Phiên bản chưa được hỗ trợ
            ValidFrom = DateTime.UtcNow.AddDays(-10),
            ExpiresAt = DateTime.UtcNow.AddYears(1)
        };

        var doc = TestLicenseSigner.CreateSignedLicenseDocument(model);
        var parsed = LicenseStorageService.ParseLicenseEnvelope(doc);
        await _storageService.SaveLicenseAsync(parsed!);

        // Act
        var result = await service.ValidateAsync();

        // Assert
        result.State.Should().Be(LicenseState.Invalid);
        result.Message.Should().Contain("Phiên bản giấy phép");
    }

    // 8. Valid offline operation
    [Fact]
    public async Task ValidateAsync_WhenValidOfflineOperationWithinAllowance_ShouldReturnActive()
    {
        // Arrange
        var service = CreateService();
        var identity = await _identityService.GetOrCreateInstallationIdentityAsync();

        var model = new LicenseModel
        {
            CustomerId = "Nhà Hàng Biển Đông",
            InstallationId = identity.InstallationId,
            ProductCode = "SmartRestaurant",
            ValidFrom = DateTime.UtcNow.AddDays(-10),
            ExpiresAt = DateTime.UtcNow.AddDays(100),
            GracePolicy = new GracePolicyDto { OfflineAllowanceDays = 30 }
        };

        var doc = TestLicenseSigner.CreateSignedLicenseDocument(model);
        await service.InstallLicenseAsync(doc);

        // Thiết lập lần kiểm thực gần nhất cách đây 15 ngày (trong hạn mức 30 ngày)
        var metadata = await _storageService.LoadStateMetadataAsync();
        metadata.LastValidatedAtUtc = DateTime.UtcNow.AddDays(-15);
        await _storageService.SaveStateMetadataAsync(metadata);

        // Act
        var result = await service.ValidateAsync();

        // Assert
        result.State.Should().Be(LicenseState.Active);
        result.CanOperate.Should().BeTrue();
        result.IsOfflineAllowanceExceeded.Should().BeFalse();
    }

    // 9. Offline allowance exceeded
    [Fact]
    public async Task ValidateAsync_WhenOfflineAllowanceExceeded_ShouldReturnOfflineGrace()
    {
        // Arrange
        var service = CreateService();
        var identity = await _identityService.GetOrCreateInstallationIdentityAsync();

        var model = new LicenseModel
        {
            CustomerId = "Nhà Hàng Biển Đông",
            InstallationId = identity.InstallationId,
            ProductCode = "SmartRestaurant",
            ValidFrom = DateTime.UtcNow.AddDays(-50),
            ExpiresAt = DateTime.UtcNow.AddDays(150),
            GracePolicy = new GracePolicyDto { OfflineAllowanceDays = 30 }
        };

        var doc = TestLicenseSigner.CreateSignedLicenseDocument(model);
        await service.InstallLicenseAsync(doc);

        // Thiết lập lần kiểm thực gần nhất cách đây 35 ngày (vượt quá hạn mức 30 ngày)
        var metadata = await _storageService.LoadStateMetadataAsync();
        metadata.LastValidatedAtUtc = DateTime.UtcNow.AddDays(-35);
        await _storageService.SaveStateMetadataAsync(metadata);

        // Act
        var result = await service.ValidateAsync();

        // Assert
        result.State.Should().Be(LicenseState.OfflineGrace);
        result.IsOfflineAllowanceExceeded.Should().BeTrue();
        result.CanOperate.Should().BeTrue(); // Đang trong ân hạn vẫn cho phép vận hành
    }

    // 10. Clock rollback detection
    [Fact]
    public async Task ValidateAsync_WhenClockRollbackDetected_ShouldReturnVerificationRequired()
    {
        // Arrange
        var service = CreateService();
        var identity = await _identityService.GetOrCreateInstallationIdentityAsync();

        var model = new LicenseModel
        {
            CustomerId = "Nhà Hàng Biển Đông",
            InstallationId = identity.InstallationId,
            ProductCode = "SmartRestaurant",
            ValidFrom = DateTime.UtcNow.AddDays(-50),
            ExpiresAt = DateTime.UtcNow.AddDays(150)
        };

        var doc = TestLicenseSigner.CreateSignedLicenseDocument(model);
        await service.InstallLicenseAsync(doc);

        // Giả lập mốc tin cậy là tương lai (ví dụ: ngày mai) do ai đó vừa lùi giờ máy tính lại ngày hôm nay
        var metadata = await _storageService.LoadStateMetadataAsync();
        metadata.LastKnownTrustedUtc = DateTime.UtcNow.AddDays(1); // Mốc tin cậy ở tương lai
        await _storageService.SaveStateMetadataAsync(metadata);

        // Act
        var result = await service.ValidateAsync();

        // Assert
        result.State.Should().Be(LicenseState.VerificationRequired);
        result.IsClockRollbackDetected.Should().BeTrue();
        result.IsValid.Should().BeFalse();
    }

    // 11. Feature entitlement checks
    [Fact]
    public async Task IsFeatureEnabled_ShouldValidateFeatureEntitlementsCorrectly()
    {
        // Arrange
        var service = CreateService();
        var identity = await _identityService.GetOrCreateInstallationIdentityAsync();

        var model = new LicenseModel
        {
            CustomerId = "Nhà Hàng Biển Đông",
            InstallationId = identity.InstallationId,
            ProductCode = "SmartRestaurant",
            Features = ["TableManagement", "KitchenKDS", "Menu"],
            ValidFrom = DateTime.UtcNow.AddDays(-10),
            ExpiresAt = DateTime.UtcNow.AddYears(1)
        };

        var doc = TestLicenseSigner.CreateSignedLicenseDocument(model);
        await service.InstallLicenseAsync(doc);

        // Act & Assert
        service.IsFeatureEnabled("TableManagement").Should().BeTrue();
        service.IsFeatureEnabled("KitchenKDS").Should().BeTrue();
        service.IsFeatureEnabled("Menu").Should().BeTrue();
        service.IsFeatureEnabled("VietQR").Should().BeFalse(); // Không có trong danh mục cấp phép
        service.IsFeatureEnabled("Inventory").Should().BeFalse();
    }

    // 12. License renewal/replacement
    [Fact]
    public async Task InstallLicenseAsync_WhenRenewingValidLicense_ShouldSucceedAndReplace()
    {
        // Arrange
        var service = CreateService();
        var identity = await _identityService.GetOrCreateInstallationIdentityAsync();

        var oldLicense = new LicenseModel
        {
            LicenseId = "OLD-LIC-001",
            CustomerId = "Nhà Hàng Biển Đông",
            InstallationId = identity.InstallationId,
            ProductCode = "SmartRestaurant",
            Edition = "Standard",
            ValidFrom = DateTime.UtcNow.AddDays(-300),
            ExpiresAt = DateTime.UtcNow.AddDays(10)
        };
        await service.InstallLicenseAsync(TestLicenseSigner.CreateSignedLicenseDocument(oldLicense));

        var renewedLicense = new LicenseModel
        {
            LicenseId = "NEW-RENEWED-LIC-002",
            CustomerId = "Nhà Hàng Biển Đông",
            InstallationId = identity.InstallationId,
            ProductCode = "SmartRestaurant",
            Edition = "Professional",
            ValidFrom = DateTime.UtcNow.AddDays(-1),
            ExpiresAt = DateTime.UtcNow.AddYears(1)
        };
        var renewedDoc = TestLicenseSigner.CreateSignedLicenseDocument(renewedLicense);

        // Act
        var result = await service.InstallLicenseAsync(renewedDoc);

        // Assert
        result.Success.Should().BeTrue();
        result.Summary!.LicenseId.Should().Be("NEW-RENEWED-LIC-002");
        result.Summary.Edition.Should().Be("Professional");

        var currentDoc = await _storageService.LoadCurrentLicenseAsync();
        currentDoc!.License.LicenseId.Should().Be("NEW-RENEWED-LIC-002");
    }

    // 13. Invalid replacement License does not overwrite a valid License
    [Fact]
    public async Task InstallLicenseAsync_WhenInvalidReplacementSubmitted_ShouldRejectAndPreserveValidExistingLicense()
    {
        // Arrange
        var service = CreateService();
        var identity = await _identityService.GetOrCreateInstallationIdentityAsync();

        // 1. Cài đặt giấy phép hợp lệ ban đầu
        var initialValidLicense = new LicenseModel
        {
            LicenseId = "INITIAL-VALID-001",
            CustomerId = "Nhà Hàng Biển Đông",
            InstallationId = identity.InstallationId,
            ProductCode = "SmartRestaurant",
            Edition = "Standard",
            ValidFrom = DateTime.UtcNow.AddDays(-10),
            ExpiresAt = DateTime.UtcNow.AddYears(1)
        };
        var validDoc = TestLicenseSigner.CreateSignedLicenseDocument(initialValidLicense);
        var initialResult = await service.InstallLicenseAsync(validDoc);
        initialResult.Success.Should().BeTrue();

        // 2. Tạo một giấy phép thay thế nhưng BỊ SAI CHỮ KÝ SỐ / CAN THIỆP DỮ LIỆU
        var candidateLicense = new LicenseModel
        {
            LicenseId = "CORRUPTED-CANDIDATE-002",
            CustomerId = "Nhà Hàng Giả Mạo",
            InstallationId = identity.InstallationId,
            ProductCode = "SmartRestaurant",
            ValidFrom = DateTime.UtcNow.AddDays(-1),
            ExpiresAt = DateTime.UtcNow.AddYears(2)
        };
        var candidateDoc = TestLicenseSigner.CreateSignedLicenseDocument(candidateLicense);

        // Làm hỏng chữ ký bằng cách tráo đổi chữ ký ngẫu nhiên
        var parsedCandidate = LicenseStorageService.ParseLicenseEnvelope(candidateDoc);
        var badCandidateDoc = LicenseStorageService.PackageLicenseEnvelope(parsedCandidate!.PayloadJson, Convert.ToBase64String(new byte[64]));

        // Act: Cố gắng nạp giấy phép bị lỗi
        var installResult = await service.InstallLicenseAsync(badCandidateDoc);

        // Assert:
        // 1. Kích hoạt giấy phép mới phải thất bại
        installResult.Success.Should().BeFalse();
        installResult.Message.Should().Contain("thất bại");

        // 2. Giấy phép hợp lệ ban đầu vẫn được bảo toàn nguyên vẹn trong kho lưu trữ
        var activeDoc = await _storageService.LoadCurrentLicenseAsync();
        activeDoc.Should().NotBeNull();
        activeDoc!.License.LicenseId.Should().Be("INITIAL-VALID-001");
        activeDoc.License.CustomerId.Should().Be("Nhà Hàng Biển Đông");

        // 3. Trạng thái kiểm thực vẫn phản ánh giấy phép hợp lệ ban đầu
        var summary = await service.GetSummaryAsync();
        summary.LicenseId.Should().Be("INITIAL-VALID-001");
        summary.State.Should().Be(LicenseState.Active);
    }

    // 14. Development Mode operation without a commercial License
    [Fact]
    public async Task ValidateAsync_InDevelopmentMode_ShouldOperateWithoutCommercialLicense()
    {
        // Arrange: Cấu hình hệ thống ở chế độ phát triển Development Mode
        var devOptions = new LicenseOptions
        {
            Mode = "Development",
            ProductCode = "SmartRestaurant",
            StorageDirectory = _testDirectory
        };
        var service = CreateService(devOptions);

        // Act: Kiểm thực trong khi KHÔNG CÓ tệp giấy phép nào được cài đặt
        var validation = await service.ValidateAsync();
        var summary = await service.GetSummaryAsync();
        var isFeatureActive = service.IsFeatureEnabled("AnyArbitraryModuleCode");

        // Assert:
        // Hệ thống hoạt động bình thường, không chặn lập trình viên
        validation.IsValid.Should().BeTrue();
        validation.State.Should().Be(LicenseState.Active);
        validation.CanOperate.Should().BeTrue();

        summary.IsDevelopmentMode.Should().BeTrue();
        summary.State.Should().Be(LicenseState.Active);
        summary.CanOperate.Should().BeTrue();

        isFeatureActive.Should().BeTrue();
    }
}
