namespace SmartRestaurant.Domain.Entities;

using System.Text.Json;
using SmartRestaurant.Domain.Common;
using SmartRestaurant.Domain.Enums;
using SmartRestaurant.Domain.ValueObjects;

/// <summary>
/// Thực thể Giấy Phép Bản Quyền Hệ Thống (System Software License)
/// Đáp ứng chuẩn thiết kế tại Chỉ thị Chủ Dự Án (Owner Directive §3.1)
/// </summary>
public class SystemLicense : BaseEntity
{
    private SystemLicense() { }

    public SystemLicense(
        string licenseId,
        string customerId,
        string installationId,
        string productCode,
        string edition,
        List<string> features,
        DateTime issuedAt,
        DateTime validFrom,
        DateTime expiresAt,
        int licenseVersion,
        GracePolicy gracePolicy,
        string rawDocument,
        string digitalSignature)
    {
        LicenseId = string.IsNullOrWhiteSpace(licenseId) ? Guid.NewGuid().ToString() : licenseId.Trim();
        CustomerId = customerId.Trim();
        InstallationId = installationId.Trim();
        ProductCode = string.IsNullOrWhiteSpace(productCode) ? "SmartRestaurant" : productCode.Trim();
        Edition = string.IsNullOrWhiteSpace(edition) ? "Standard" : edition.Trim();
        FeaturesJson = JsonSerializer.Serialize(features ?? []);
        IssuedAt = DateTime.SpecifyKind(issuedAt, DateTimeKind.Utc);
        ValidFrom = DateTime.SpecifyKind(validFrom, DateTimeKind.Utc);
        ExpiresAt = DateTime.SpecifyKind(expiresAt, DateTimeKind.Utc);
        LicenseVersion = licenseVersion <= 0 ? 1 : licenseVersion;
        GracePolicy = gracePolicy ?? GracePolicy.Default;
        RawDocument = rawDocument ?? string.Empty;
        DigitalSignature = digitalSignature ?? string.Empty;
        InstalledAt = DateTime.UtcNow;
        IsActive = true;
    }

    public string LicenseId { get; private set; } = string.Empty;
    public string CustomerId { get; private set; } = string.Empty;
    public string InstallationId { get; private set; } = string.Empty;
    public string ProductCode { get; private set; } = "SmartRestaurant";
    public string Edition { get; private set; } = "Standard";
    public string FeaturesJson { get; private set; } = "[]";
    public DateTime IssuedAt { get; private set; }
    public DateTime ValidFrom { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public int LicenseVersion { get; private set; } = 1;
    public GracePolicy GracePolicy { get; private set; } = GracePolicy.Default;
    public string RawDocument { get; private set; } = string.Empty;
    public string DigitalSignature { get; private set; } = string.Empty;
    public DateTime InstalledAt { get; private set; }
    public DateTime? LastValidatedAt { get; private set; }
    public bool IsActive { get; private set; } = true;

    /// <summary>
    /// Đánh giá trạng thái bản quyền hiện tại theo các tiêu chuẩn của Owner Directive (§3.4 & §3.5)
    /// </summary>
    public LicenseState EvaluateState(
        string currentInstallationId,
        DateTime currentUtc,
        DateTime? lastValidatedUtc = null,
        bool clockRollbackDetected = false)
    {
        if (!IsActive)
        {
            return LicenseState.Uninitialized;
        }

        if (clockRollbackDetected)
        {
            return LicenseState.VerificationRequired;
        }

        // 1. Kiểm tra ràng buộc InstallationId của máy chủ
        if (!string.IsNullOrWhiteSpace(InstallationId) &&
            !string.Equals(InstallationId, "*", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(InstallationId, currentInstallationId, StringComparison.OrdinalIgnoreCase))
        {
            return LicenseState.Invalid;
        }

        // 2. Kiểm tra thời điểm bắt đầu hiệu lực (ValidFrom)
        if (currentUtc < ValidFrom)
        {
            return LicenseState.Invalid;
        }

        // 3. Kiểm tra hạn sử dụng (ExpiresAt)
        var expiryGraceEnd = ExpiresAt.AddDays(GracePolicy.ExpiryGraceDays);
        if (currentUtc > expiryGraceEnd)
        {
            return LicenseState.Expired;
        }

        if (currentUtc > ExpiresAt)
        {
            return LicenseState.ExpiredGrace;
        }

        // 4. Kiểm tra thời gian ân hạn sắp hết hạn (ExpiringSoon)
        var expiringSoonThreshold = ExpiresAt.AddDays(-GracePolicy.ExpiringSoonDays);
        if (currentUtc >= expiringSoonThreshold)
        {
            return LicenseState.ExpiringSoon;
        }

        // 5. Kiểm tra thời gian ngoại tuyến tối đa cho phép (Offline Allowance)
        var checkValidationTime = lastValidatedUtc ?? LastValidatedAt ?? InstalledAt;
        if ((currentUtc - checkValidationTime).TotalDays > GracePolicy.OfflineAllowanceDays)
        {
            return LicenseState.OfflineGrace;
        }

        return LicenseState.Active;
    }

    /// <summary>
    /// Kiểm tra quyền sở hữu tính năng / phân hệ (Feature Entitlement)
    /// </summary>
    public bool IsFeatureEnabled(string featureCode)
    {
        if (string.IsNullOrWhiteSpace(featureCode))
        {
            return false;
        }

        try
        {
            var features = JsonSerializer.Deserialize<List<string>>(FeaturesJson);
            if (features == null || features.Count == 0)
            {
                return false;
            }

            return features.Any(f => f.Equals("*", StringComparison.OrdinalIgnoreCase) ||
                                     f.Equals(featureCode, StringComparison.OrdinalIgnoreCase));
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Lấy danh sách các tính năng được cấp phép
    /// </summary>
    public List<string> GetFeatures()
    {
        try
        {
            return JsonSerializer.Deserialize<List<string>>(FeaturesJson) ?? [];
        }
        catch
        {
            return [];
        }
    }

    /// <summary>
    /// Ghi nhận thời điểm xác thực thành công gần nhất
    /// </summary>
    public void RecordValidation(DateTime validatedAtUtc)
    {
        LastValidatedAt = DateTime.SpecifyKind(validatedAtUtc, DateTimeKind.Utc);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Hủy kích hoạt giấy phép cũ khi có giấy phép mới thay thế
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
