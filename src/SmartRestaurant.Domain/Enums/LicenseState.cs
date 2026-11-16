namespace SmartRestaurant.Domain.Enums;

/// <summary>
/// Trạng thái vòng đời giấy phép bản quyền phần mềm SmartRestaurant
/// Tuân thủ quy định quản trị tại Chỉ thị Chủ Dự Án (Owner Directive §3.4)
/// </summary>
public enum LicenseState
{
    /// <summary>
    /// Hệ thống chưa được nạp hoặc khởi tạo giấy phép bản quyền
    /// </summary>
    Uninitialized = 0,

    /// <summary>
    /// Giấy phép hợp lệ, đang trong thời hạn hiệu lực và vận hành bình thường
    /// </summary>
    Active = 1,

    /// <summary>
    /// Giấy phép sắp hết hạn (trong khung cảnh báo gia hạn, ví dụ còn <= 14 ngày)
    /// </summary>
    ExpiringSoon = 2,

    /// <summary>
    /// Đang vận hành ngoại tuyến trong thời gian ân hạn cho phép (Offline Grace Period)
    /// </summary>
    OfflineGrace = 3,

    /// <summary>
    /// Giấy phép đã hết hạn chính thức nhưng đang trong chu kỳ ân hạn vận hành (Expiry Grace Period)
    /// </summary>
    ExpiredGrace = 4,

    /// <summary>
    /// Giấy phép đã hết hạn toàn bộ (vượt quá cả chu kỳ ân hạn)
    /// </summary>
    Expired = 5,

    /// <summary>
    /// Cần thực hiện kiểm thực lại (do phát hiện nghi vấn quay lùi đồng hồ hệ thống hoặc cần đồng bộ)
    /// </summary>
    VerificationRequired = 6,

    /// <summary>
    /// Giấy phép không hợp lệ (sai chữ ký số, sai InstallationId, sai phiên bản, hoặc dữ liệu bị biến dạng)
    /// </summary>
    Invalid = 7,

    /// <summary>
    /// Giấy phép bị tạm đình chỉ hoạt động
    /// </summary>
    Suspended = 8
}
