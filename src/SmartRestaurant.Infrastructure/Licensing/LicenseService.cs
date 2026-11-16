namespace SmartRestaurant.Infrastructure.Licensing;

using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartRestaurant.Application.DTOs;
using SmartRestaurant.Application.Interfaces;
using SmartRestaurant.Domain.Enums;

public class LicenseService(
    ILicenseSignatureValidator signatureValidator,
    IInstallationIdentityService installationIdentityService,
    ILicenseStorageService storageService,
    ILicenseClockService clockService,
    IOptions<LicenseOptions> options,
    ILogger<LicenseService> logger) : ILicenseService
{
    private readonly ILicenseSignatureValidator _signatureValidator = signatureValidator;
    private readonly IInstallationIdentityService _installationIdentityService = installationIdentityService;
    private readonly ILicenseStorageService _storageService = storageService;
    private readonly ILicenseClockService _clockService = clockService;
    private readonly LicenseOptions _options = options.Value;
    private readonly ILogger<LicenseService> _logger = logger;
    private LicenseState _currentState = LicenseState.Uninitialized;

    public LicenseState CurrentState => _currentState;

    public async Task<LicenseValidationResult> ValidateAsync(CancellationToken cancellationToken = default)
    {
        var identity = await _installationIdentityService.GetOrCreateInstallationIdentityAsync(cancellationToken);
        var now = _clockService.UtcNow;

        // 1. Chế độ phát triển (Development Mode) — Không đòi hỏi License Server thương mại
        if (string.Equals(_options.Mode, "Development", StringComparison.OrdinalIgnoreCase))
        {
            _currentState = LicenseState.Active;
            return new LicenseValidationResult
            {
                IsValid = true,
                State = LicenseState.Active,
                StateName = "Chế Độ Phát Triển (Development Mode)",
                Message = "Hệ thống đang chạy ở chế độ phát triển nội bộ; toàn bộ tính năng và phân hệ được mở không hạn chế.",
                DaysRemaining = 365,
                GraceDaysRemaining = 14,
                OfflineDaysRemaining = 30,
                IsClockRollbackDetected = false,
                IsOfflineAllowanceExceeded = false
            };
        }

        // 2. Chế độ sản xuất (Production Mode) — Kiểm thực nghiêm ngặt
        var doc = await _storageService.LoadCurrentLicenseAsync(cancellationToken);
        if (doc == null || doc.License == null)
        {
            _currentState = LicenseState.Uninitialized;
            return new LicenseValidationResult
            {
                IsValid = false,
                State = LicenseState.Uninitialized,
                StateName = "Chưa Khởi Tạo (Uninitialized)",
                Message = "Hệ thống chưa được cài đặt giấy phép bản quyền chính thức."
            };
        }

        var metadata = await _storageService.LoadStateMetadataAsync(cancellationToken);

        // Kiểm tra an toàn đồng hồ (Clock Sanity / Rollback Detection)
        var tolerance = TimeSpan.FromMinutes(_options.ClockDriftToleranceMinutes <= 0 ? 10 : _options.ClockDriftToleranceMinutes);
        var isClockSane = _clockService.ValidateClockSanity(now, metadata.LastKnownTrustedUtc, tolerance);

        if (!isClockSane)
        {
            _logger.LogWarning("Phát hiện bất thường lùi thời gian hệ thống! Hiện tại: {Now:u}, Mốc tin cậy gần nhất: {Trusted:u}",
                now, metadata.LastKnownTrustedUtc);

            _currentState = LicenseState.VerificationRequired;
            return new LicenseValidationResult
            {
                IsValid = false,
                State = LicenseState.VerificationRequired,
                StateName = "Cần Xác Thực Lại (Verification Required)",
                Message = "Phát hiện thời gian hệ thống bị lùi so với mốc tin cậy gần nhất. Vui lòng kiểm tra lại đồng hồ máy chủ.",
                IsClockRollbackDetected = true
            };
        }

        // Cập nhật mốc thời gian tin cậy mới nếu tiến về phía trước
        if (now > metadata.LastKnownTrustedUtc)
        {
            metadata.LastKnownTrustedUtc = now;
        }

        // Kiểm tra chữ ký số ECDSA P-256
        var isSignatureValid = _signatureValidator.VerifySignature(doc.PayloadJson, doc.SignatureBase64);
        if (!isSignatureValid)
        {
            _logger.LogWarning("Kiểm thực thất bại: Chữ ký số bản quyền ECDSA P-256 không hợp lệ.");
            _currentState = LicenseState.Invalid;
            return new LicenseValidationResult
            {
                IsValid = false,
                State = LicenseState.Invalid,
                StateName = "Không Hợp Lệ (Invalid)",
                Message = "Chữ ký số ECDSA P-256 của giấy phép không hợp lệ hoặc dữ liệu bị can thiệp trái phép."
            };
        }

        var license = doc.License;

        // Kiểm tra mã sản phẩm
        if (!string.Equals(license.ProductCode, _options.ProductCode, StringComparison.OrdinalIgnoreCase))
        {
            _currentState = LicenseState.Invalid;
            return new LicenseValidationResult
            {
                IsValid = false,
                State = LicenseState.Invalid,
                StateName = "Không Hợp Lệ (Invalid)",
                Message = $"Mã sản phẩm trong giấy phép ('{license.ProductCode}') không khớp với hệ thống ('{_options.ProductCode}')."
            };
        }

        // Kiểm tra phiên bản giấy phép
        if (license.LicenseVersion != 1)
        {
            _currentState = LicenseState.Invalid;
            return new LicenseValidationResult
            {
                IsValid = false,
                State = LicenseState.Invalid,
                StateName = "Không Hợp Lệ (Invalid)",
                Message = $"Phiên bản giấy phép ({license.LicenseVersion}) không được hỗ trợ."
            };
        }

        // Kiểm tra ràng buộc InstallationId
        if (!string.IsNullOrWhiteSpace(license.InstallationId) &&
            !string.Equals(license.InstallationId, "*", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(license.InstallationId, identity.InstallationId, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("InstallationId không khớp. License: {LicInstall}, Host: {HostInstall}",
                license.InstallationId, identity.InstallationId);

            _currentState = LicenseState.Invalid;
            return new LicenseValidationResult
            {
                IsValid = false,
                State = LicenseState.Invalid,
                StateName = "Không Hợp Lệ (Invalid)",
                Message = "Giấy phép bản quyền không khớp với mã định danh máy chủ (InstallationId) của hệ thống này."
            };
        }

        // Kiểm tra thời điểm bắt đầu hiệu lực (ValidFrom)
        if (now < license.ValidFrom)
        {
            _currentState = LicenseState.Invalid;
            return new LicenseValidationResult
            {
                IsValid = false,
                State = LicenseState.Invalid,
                StateName = "Chưa Có Hiệu Lực",
                Message = $"Giấy phép chưa đến thời điểm có hiệu lực (Bắt đầu từ: {license.ValidFrom:dd/MM/yyyy HH:mm:ss} UTC)."
            };
        }

        // Đọc các giá trị chính sách ân hạn từ giấy phép hoặc cấu hình
        var grace = license.GracePolicy ?? new GracePolicyDto();
        var offlineAllowanceDays = grace.OfflineAllowanceDays > 0 ? grace.OfflineAllowanceDays : _options.DefaultOfflineAllowanceDays;
        var expiryGraceDays = grace.ExpiryGraceDays >= 0 ? grace.ExpiryGraceDays : _options.DefaultExpiryGraceDays;
        var expiringSoonDays = grace.ExpiringSoonDays > 0 ? grace.ExpiringSoonDays : _options.ExpiringSoonDays;

        var daysRemaining = (int)Math.Max(0, (license.ExpiresAt.Date - now.Date).TotalDays);
        int? graceRemaining = null;
        if (now > license.ExpiresAt)
        {
            var graceEndDate = license.ExpiresAt.AddDays(expiryGraceDays);
            graceRemaining = (int)Math.Max(0, (graceEndDate.Date - now.Date).TotalDays);
        }

        // Kiểm tra chu kỳ ngoại tuyến tối đa (Offline Allowance)
        int? offlineDaysRemaining = null;
        var isOfflineAllowanceExceeded = false;
        if (metadata.LastValidatedAtUtc.HasValue)
        {
            var offlineElapsedDays = (now - metadata.LastValidatedAtUtc.Value).TotalDays;
            offlineDaysRemaining = (int)Math.Max(0, offlineAllowanceDays - offlineElapsedDays);
            if (offlineElapsedDays > offlineAllowanceDays)
            {
                isOfflineAllowanceExceeded = true;
            }
        }
        else
        {
            offlineDaysRemaining = offlineAllowanceDays;
        }

        // Xác định trạng thái cuối cùng
        var expiryGraceLimit = license.ExpiresAt.AddDays(expiryGraceDays);
        LicenseState finalState;
        string finalMessage;

        if (now > expiryGraceLimit)
        {
            finalState = LicenseState.Expired;
            finalMessage = "Giấy phép bản quyền đã hết hạn sử dụng toàn bộ (kể cả thời gian ân hạn).";
        }
        else if (now > license.ExpiresAt)
        {
            finalState = LicenseState.ExpiredGrace;
            finalMessage = $"Giấy phép đã hết hạn chính thức nhưng đang trong thời gian ân hạn vận hành (Còn {graceRemaining} ngày).";
        }
        else if (isOfflineAllowanceExceeded)
        {
            finalState = LicenseState.OfflineGrace;
            finalMessage = $"Thời gian vận hành ngoại tuyến đã vượt quá hạn mức cho phép ({offlineAllowanceDays} ngày).";
        }
        else if (now >= license.ExpiresAt.AddDays(-expiringSoonDays))
        {
            finalState = LicenseState.ExpiringSoon;
            finalMessage = $"Giấy phép sắp hết hạn (Còn {daysRemaining} ngày). Vui lòng liên hệ gia hạn.";
        }
        else
        {
            finalState = LicenseState.Active;
            finalMessage = "Giấy phép bản quyền hợp lệ và đang hoạt động bình thường.";
        }

        _currentState = finalState;

        // Ghi nhận thời điểm xác thực thành công vào metadata
        if (finalState is LicenseState.Active or LicenseState.ExpiringSoon)
        {
            metadata.LastValidatedAtUtc = now;
            metadata.ValidationCount++;
            metadata.ActiveLicenseId = license.LicenseId;
            await _storageService.SaveStateMetadataAsync(metadata, cancellationToken);
        }

        return new LicenseValidationResult
        {
            IsValid = finalState is LicenseState.Active or LicenseState.ExpiringSoon or LicenseState.OfflineGrace or LicenseState.ExpiredGrace,
            State = finalState,
            StateName = MapStateName(finalState),
            Message = finalMessage,
            DaysRemaining = daysRemaining,
            GraceDaysRemaining = graceRemaining,
            OfflineDaysRemaining = offlineDaysRemaining,
            IsClockRollbackDetected = false,
            IsOfflineAllowanceExceeded = isOfflineAllowanceExceeded
        };
    }

    public async Task<LicenseSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        var identity = await _installationIdentityService.GetOrCreateInstallationIdentityAsync(cancellationToken);
        var validation = await ValidateAsync(cancellationToken);

        if (string.Equals(_options.Mode, "Development", StringComparison.OrdinalIgnoreCase))
        {
            return new LicenseSummaryDto
            {
                State = LicenseState.Active,
                StateText = "Bản Quyền Chế Độ Phát Triển (Development)",
                StateBadgeClass = "badge bg-info text-dark",
                LicenseId = "DEV-LIC-00000000",
                CustomerId = "NHÀ HÀNG PHÁT TRIỂN NỘI BỘ",
                InstallationId = identity.InstallationId,
                ProductCode = "SmartRestaurant",
                Edition = "Development Unlimited",
                Features = ["*"],
                IssuedAt = DateTime.UtcNow.AddDays(-30),
                ValidFrom = DateTime.UtcNow.AddDays(-30),
                ExpiresAt = DateTime.UtcNow.AddYears(10),
                LicenseVersion = 1,
                DaysRemaining = 3650,
                IsDevelopmentMode = true,
                InstallationIdentity = identity
            };
        }

        var doc = await _storageService.LoadCurrentLicenseAsync(cancellationToken);
        if (doc == null || doc.License == null)
        {
            return new LicenseSummaryDto
            {
                State = LicenseState.Uninitialized,
                StateText = "Chưa nạp giấy phép",
                StateBadgeClass = "badge bg-secondary",
                InstallationId = identity.InstallationId,
                InstallationIdentity = identity,
                IsDevelopmentMode = false
            };
        }

        var lic = doc.License;
        return new LicenseSummaryDto
        {
            State = validation.State,
            StateText = validation.Message,
            StateBadgeClass = MapStateBadgeClass(validation.State),
            LicenseId = lic.LicenseId,
            CustomerId = lic.CustomerId,
            InstallationId = lic.InstallationId,
            ProductCode = lic.ProductCode,
            Edition = lic.Edition,
            Features = lic.Features ?? [],
            IssuedAt = lic.IssuedAt,
            ValidFrom = lic.ValidFrom,
            ExpiresAt = lic.ExpiresAt,
            LicenseVersion = lic.LicenseVersion,
            DaysRemaining = validation.DaysRemaining,
            GraceDaysRemaining = validation.GraceDaysRemaining,
            OfflineDaysRemaining = validation.OfflineDaysRemaining,
            IsDevelopmentMode = false,
            InstallationIdentity = identity
        };
    }

    public async Task<LicenseActivationResultDto> InstallLicenseAsync(string licenseDocument, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(licenseDocument))
        {
            return new LicenseActivationResultDto
            {
                Success = false,
                State = _currentState,
                Message = "Nội dung tệp giấy phép bản quyền không được để trống.",
                Errors = ["Vui lòng chọn tệp .lic hợp lệ hoặc dán chuỗi bản quyền."]
            };
        }

        // 1. Phân giải tệp giấy phép
        var candidateDoc = LicenseStorageService.ParseLicenseEnvelope(licenseDocument);
        if (candidateDoc == null || candidateDoc.License == null)
        {
            return new LicenseActivationResultDto
            {
                Success = false,
                State = _currentState,
                Message = "Tệp giấy phép bị sai định dạng hoặc không thể giải mã.",
                Errors = ["Cấu trúc tệp không theo chuẩn envelope -----BEGIN SMARTRESTAURANT LICENSE-----."]
            };
        }

        // 2. Kiểm thực chữ ký số ECDSA P-256 của giấy phép mới
        // NGUYÊN TẮC: Chữ ký không hợp lệ phải bị từ chối ngay và KHÔNG được ghi đè giấy phép cũ!
        var isSignatureValid = _signatureValidator.VerifySignature(candidateDoc.PayloadJson, candidateDoc.SignatureBase64);
        if (!isSignatureValid)
        {
            _logger.LogWarning("Từ chối nạp giấy phép mới: Chữ ký số ECDSA P-256 không hợp lệ.");
            return new LicenseActivationResultDto
            {
                Success = false,
                State = _currentState,
                Message = "Kích hoạt thất bại: Chữ ký số mã hóa ECDSA P-256 không hợp lệ.",
                Errors = ["Tệp giấy phép không được phát hành bởi Nhà cung cấp chính thức hoặc đã bị chỉnh sửa dữ liệu."]
            };
        }

        var candidate = candidateDoc.License;
        var identity = await _installationIdentityService.GetOrCreateInstallationIdentityAsync(cancellationToken);

        // 3. Kiểm tra ProductCode
        if (!string.Equals(candidate.ProductCode, _options.ProductCode, StringComparison.OrdinalIgnoreCase))
        {
            return new LicenseActivationResultDto
            {
                Success = false,
                State = _currentState,
                Message = "Kích hoạt thất bại: Mã sản phẩm không phù hợp.",
                Errors = [$"Giấy phép dành cho sản phẩm '{candidate.ProductCode}', hệ thống yêu cầu '{_options.ProductCode}'."]
            };
        }

        // 4. Kiểm tra phiên bản giấy phép
        if (candidate.LicenseVersion != 1)
        {
            return new LicenseActivationResultDto
            {
                Success = false,
                State = _currentState,
                Message = "Kích hoạt thất bại: Phiên bản giấy phép không được hỗ trợ.",
                Errors = [$"Phiên bản giấy phép {candidate.LicenseVersion} không tương thích."]
            };
        }

        // 5. Kiểm tra InstallationId ràng buộc máy chủ
        if (!string.IsNullOrWhiteSpace(candidate.InstallationId) &&
            !string.Equals(candidate.InstallationId, "*", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(candidate.InstallationId, identity.InstallationId, StringComparison.OrdinalIgnoreCase))
        {
            return new LicenseActivationResultDto
            {
                Success = false,
                State = _currentState,
                Message = "Kích hoạt thất bại: Giấy phép được cấp riêng cho một máy chủ khác.",
                Errors = [
                    $"InstallationId của giấy phép: {candidate.InstallationId}",
                    $"InstallationId của máy chủ này: {identity.InstallationId}"
                ]
            };
        }

        // 6. Kiểm tra hạn sử dụng (Nếu giấy phép đã hết hạn ngay từ khi nạp)
        var now = _clockService.UtcNow;
        var grace = candidate.GracePolicy ?? new GracePolicyDto();
        var expiryGraceDays = grace.ExpiryGraceDays >= 0 ? grace.ExpiryGraceDays : _options.DefaultExpiryGraceDays;
        if (now > candidate.ExpiresAt.AddDays(expiryGraceDays))
        {
            return new LicenseActivationResultDto
            {
                Success = false,
                State = _currentState,
                Message = "Kích hoạt thất bại: Giấy phép này đã hết hạn sử dụng.",
                Errors = [$"Ngày hết hạn ghi nhận: {candidate.ExpiresAt:dd/MM/yyyy HH:mm:ss} UTC."]
            };
        }

        // 7. Tất cả kiểm tra đều HỢP LỆ -> Thực hiện lưu trữ nguyên tử (Atomic Replace)
        await _storageService.SaveLicenseAsync(candidateDoc, cancellationToken);

        var metadata = await _storageService.LoadStateMetadataAsync(cancellationToken);
        metadata.LastValidatedAtUtc = now;
        metadata.LastKnownTrustedUtc = now > metadata.LastKnownTrustedUtc ? now : metadata.LastKnownTrustedUtc;
        metadata.ValidationCount++;
        metadata.ActiveLicenseId = candidate.LicenseId;
        await _storageService.SaveStateMetadataAsync(metadata, cancellationToken);

        _logger.LogInformation("Kích hoạt thành công giấy phép bản quyền {LicenseId} cho khách hàng {CustomerId}.",
            candidate.LicenseId, candidate.CustomerId);

        var summary = await GetSummaryAsync(cancellationToken);
        return new LicenseActivationResultDto
        {
            Success = true,
            State = summary.State,
            Message = $"Kích hoạt bản quyền thành công cho đơn vị {candidate.CustomerId}!",
            Summary = summary
        };
    }

    public bool IsFeatureEnabled(string featureCode)
    {
        if (string.IsNullOrWhiteSpace(featureCode))
        {
            return false;
        }

        // Ở chế độ phát triển (Development Mode), toàn bộ tính năng đều được kích hoạt
        if (string.Equals(_options.Mode, "Development", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        var doc = _storageService.LoadCurrentLicenseAsync().GetAwaiter().GetResult();
        if (doc == null || doc.License == null)
        {
            return false;
        }

        var features = doc.License.Features;
        if (features == null || features.Count == 0)
        {
            return false;
        }

        return features.Any(f => f.Equals("*", StringComparison.OrdinalIgnoreCase) ||
                                 f.Equals(featureCode, StringComparison.OrdinalIgnoreCase));
    }

    public Task<InstallationIdentityDto> GetInstallationIdentityAsync(CancellationToken cancellationToken = default)
    {
        return _installationIdentityService.GetOrCreateInstallationIdentityAsync(cancellationToken);
    }

    private static string MapStateName(LicenseState state)
    {
        return state switch
        {
            LicenseState.Active => "Hợp Lệ (Active)",
            LicenseState.ExpiringSoon => "Sắp Hết Hạn (Expiring Soon)",
            LicenseState.OfflineGrace => "Ân Hạn Ngoại Tuyến (Offline Grace)",
            LicenseState.ExpiredGrace => "Ân Hạn Hết Hạn (Expired Grace)",
            LicenseState.Expired => "Đã Hết Hạn (Expired)",
            LicenseState.VerificationRequired => "Cần Xác Thực Lại (Verification Required)",
            LicenseState.Invalid => "Không Hợp Lệ (Invalid)",
            LicenseState.Suspended => "Bị Tạm Đình Chỉ (Suspended)",
            _ => "Chưa Khởi Tạo (Uninitialized)"
        };
    }

    private static string MapStateBadgeClass(LicenseState state)
    {
        return state switch
        {
            LicenseState.Active => "badge bg-success",
            LicenseState.ExpiringSoon => "badge bg-warning text-dark",
            LicenseState.OfflineGrace => "badge bg-warning text-dark",
            LicenseState.ExpiredGrace => "badge bg-danger",
            LicenseState.Expired => "badge bg-danger",
            LicenseState.VerificationRequired => "badge bg-danger",
            LicenseState.Invalid => "badge bg-danger",
            LicenseState.Suspended => "badge bg-dark text-white",
            _ => "badge bg-secondary"
        };
    }
}
