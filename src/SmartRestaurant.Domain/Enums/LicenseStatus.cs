namespace SmartRestaurant.Domain.Enums;

/// <summary>
/// Trạng thái kiểm thực giấy phép bản quyền hệ thống
/// </summary>
public enum LicenseStatus
{
    /// <summary>
    /// Chưa cài đặt bản quyền trong hệ thống
    /// </summary>
    Unlicensed = 0,

    /// <summary>
    /// Bản quyền hợp lệ và đang hoạt động đầy đủ quyền
    /// </summary>
    Active = 1,

    /// <summary>
    /// Giấy phép hết hạn nhưng đang trong thời gian ân hạn cho phép vận hành kèm cảnh báo
    /// </summary>
    GracePeriod = 2,

    /// <summary>
    /// Giấy phép đã hết hạn toàn bộ (kể cả thời gian ân hạn)
    /// </summary>
    Expired = 3,

    /// <summary>
    /// Mã định danh phần cứng máy chủ không khớp với giấy phép
    /// </summary>
    MachineMismatch = 4,

    /// <summary>
    /// Chữ ký số mã hóa không hợp lệ hoặc dữ liệu giấy phép bị can thiệp/sửa đổi
    /// </summary>
    InvalidSignature = 5,

    /// <summary>
    /// Tính năng/phân hệ yêu cầu không nằm trong danh mục được cấp phép
    /// </summary>
    FeatureNotLicensed = 6,

    /// <summary>
    /// Số lượng bàn hoạt động đã vượt quá hạn mức cấp phép tối đa
    /// </summary>
    TableLimitExceeded = 7
}
