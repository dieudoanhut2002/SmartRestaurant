namespace SmartRestaurant.Application.Interfaces;

using SmartRestaurant.Application.DTOs;

/// <summary>
/// Giao diện lưu trữ an toàn giấy phép bản quyền cục bộ và siêu dữ liệu kiểm thực
/// Tuân thủ quy định tại Chỉ thị Chủ Dự Án (Owner Directive §3.6)
/// </summary>
public interface ILicenseStorageService
{
    /// <summary>
    /// Đọc giấy phép bản quyền đang hoạt động
    /// </summary>
    Task<StoredLicenseDocument?> LoadCurrentLicenseAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Lưu trữ an toàn giấy phép mới (sử dụng cơ chế ghi nguyên tử, bảo đảm giấy phép hợp lệ cũ không bị phá hủy nếu lỗi)
    /// </summary>
    Task SaveLicenseAsync(StoredLicenseDocument document, CancellationToken cancellationToken = default);

    /// <summary>
    /// Đọc siêu dữ liệu kiểm thực cục bộ (LastValidatedAt, LastKnownTrustedUtc)
    /// </summary>
    Task<LicenseStateMetadata> LoadStateMetadataAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Lưu trữ siêu dữ liệu kiểm thực cục bộ có bảo vệ an toàn hệ điều hành (DPAPI)
    /// </summary>
    Task SaveStateMetadataAsync(LicenseStateMetadata metadata, CancellationToken cancellationToken = default);
}
