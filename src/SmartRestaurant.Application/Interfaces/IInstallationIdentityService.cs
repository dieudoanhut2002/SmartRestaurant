namespace SmartRestaurant.Application.Interfaces;

using SmartRestaurant.Application.DTOs;

/// <summary>
/// Giao diện định danh cài đặt máy chủ nhà hàng (Installation Identity)
/// Tuân thủ quy định tại Chỉ thị Chủ Dự Án (Owner Directive §3.2)
/// </summary>
public interface IInstallationIdentityService
{
    /// <summary>
    /// Lấy hoặc tạo mới mã định danh cài đặt ngẫu nhiên bền vững (InstallationId) và cặp khóa máy chủ
    /// </summary>
    Task<InstallationIdentityDto> GetOrCreateInstallationIdentityAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy mã định danh phần cứng máy chủ (Hardware Fingerprint) đa tín hiệu
    /// </summary>
    Task<string> GetHardwareFingerprintAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Xuất gói yêu cầu cấp đổi/chuyển dịch bản quyền sang máy chủ mới (Rehost License Request)
    /// </summary>
    Task<string> ExportRehostRequestAsync(CancellationToken cancellationToken = default);
}
