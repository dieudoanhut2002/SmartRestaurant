namespace SmartRestaurant.Application.Interfaces;

using SmartRestaurant.Application.DTOs;
using SmartRestaurant.Domain.Enums;

/// <summary>
/// Giao diện điều phối nghiệp vụ bản quyền hệ thống SmartRestaurant
/// Tuân thủ quy định tại Chỉ thị Chủ Dự Án (Owner Directive §3.4 & §3.7)
/// </summary>
public interface ILicenseService
{
    /// <summary>
    /// Trạng thái bản quyền hiện thời của hệ thống
    /// </summary>
    LicenseState CurrentState { get; }

    /// <summary>
    /// Thực hiện chu trình kiểm thực bản quyền toàn diện (Chữ ký, Hạn dùng, Ân hạn, Đồng hồ, InstallationId)
    /// </summary>
    Task<LicenseValidationResult> ValidateAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy tóm tắt thông tin giấy phép phục vụ hiển thị trên giao diện Cổng Quản Lý
    /// </summary>
    Task<LicenseSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Nạp và kích hoạt giấy phép mới từ văn bản ký số (.lic).
    /// BẢO ĐẢM: Giấy phép mới không hợp lệ sẽ bị từ chối và KHÔNG phá hủy giấy phép hợp lệ cũ đang hoạt động.
    /// </summary>
    Task<LicenseActivationResultDto> InstallLicenseAsync(string licenseDocument, CancellationToken cancellationToken = default);

    /// <summary>
    /// Kiểm tra xem một tính năng / phân hệ có được cấp phép vận hành hay không (Feature Entitlement Foundation)
    /// </summary>
    bool IsFeatureEnabled(string featureCode);

    /// <summary>
    /// Lấy thông tin định danh cài đặt máy chủ (InstallationId, Hardware Fingerprint)
    /// </summary>
    Task<InstallationIdentityDto> GetInstallationIdentityAsync(CancellationToken cancellationToken = default);
}
