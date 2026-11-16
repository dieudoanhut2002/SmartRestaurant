namespace SmartRestaurant.Domain.Enums;

/// <summary>
/// Loại hình bản quyền phần mềm SmartRestaurant
/// </summary>
public enum LicenseType
{
    /// <summary>
    /// Bản quyền thử nghiệm / Đánh giá giới hạn thời gian (Trial)
    /// </summary>
    Trial = 0,

    /// <summary>
    /// Bản quyền thương mại tiêu chuẩn (Standard Commercial)
    /// </summary>
    Standard = 1,

    /// <summary>
    /// Bản quyền thương mại chuyên nghiệp (Professional Commercial)
    /// </summary>
    Professional = 2,

    /// <summary>
    /// Bản quyền doanh nghiệp không giới hạn (Enterprise Unlimited)
    /// </summary>
    Enterprise = 3
}
