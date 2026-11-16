namespace SmartRestaurant.Domain.ValueObjects;

/// <summary>
/// Chính sách ân hạn bản quyền ngoại tuyến và hết hạn
/// Không hardcode rải rác mà được định nghĩa tập trung và có thể cấu hình
/// </summary>
public record GracePolicy
{
    public GracePolicy(int offlineAllowanceDays = 30, int expiryGraceDays = 7, int expiringSoonDays = 14)
    {
        OfflineAllowanceDays = offlineAllowanceDays < 0 ? 0 : offlineAllowanceDays;
        ExpiryGraceDays = expiryGraceDays < 0 ? 0 : expiryGraceDays;
        ExpiringSoonDays = expiringSoonDays < 0 ? 0 : expiringSoonDays;
    }

    /// <summary>
    /// Số ngày tối đa cho phép hệ thống hoạt động ngoại tuyến không cần đồng bộ (mặc định 30 ngày)
    /// </summary>
    public int OfflineAllowanceDays { get; init; } = 30;

    /// <summary>
    /// Số ngày ân hạn sau khi giấy phép hết hạn trước khi ngừng hoạt động (mặc định 7 ngày)
    /// </summary>
    public int ExpiryGraceDays { get; init; } = 7;

    /// <summary>
    /// Khung thời gian trước khi hết hạn bắt đầu hiển thị thông báo cảnh báo sắp hết hạn (mặc định 14 ngày)
    /// </summary>
    public int ExpiringSoonDays { get; init; } = 14;

    public static GracePolicy Default => new(offlineAllowanceDays: 30, expiryGraceDays: 7, expiringSoonDays: 14);
}
