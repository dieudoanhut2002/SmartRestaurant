namespace SmartRestaurant.Application.Interfaces;

/// <summary>
/// Dịch vụ quản lý thời gian và phát hiện bất thường lùi đồng hồ hệ thống (Clock Rollback Detection)
/// Tuân thủ quy định tại Chỉ thị Chủ Dự Án (Owner Directive §3.5)
/// </summary>
public interface ILicenseClockService
{
    /// <summary>
    /// Thời gian hiện tại theo chuẩn UTC
    /// </summary>
    DateTime UtcNow { get; }

    /// <summary>
    /// Xác thực tính hợp lý của đồng hồ hệ thống so với mốc thời gian tin cậy gần nhất (LastKnownTrustedUtc)
    /// Trả về false nếu phát hiện đồng hồ bị lùi vượt quá ngưỡng dung sai cho phép (Tolerance)
    /// </summary>
    bool ValidateClockSanity(DateTime currentTimeUtc, DateTime lastKnownTrustedUtc, TimeSpan tolerance);
}
