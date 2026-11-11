namespace SmartRestaurant.Domain.Enums;

public enum TableStatus
{
    Available = 0,        // Trống (Xanh lá)
    Occupied = 1,         // Có khách (Vàng)
    Reserved = 2,         // Đã đặt trước (Xanh dương)
    CleaningNeeded = 3,   // Cần dọn dẹp (Cam)
    BillingRequested = 4, // Đang thanh toán / Tạm tính (Tím)
    OutOfService = 5      // Ngừng phục vụ / Bảo trì (Xám)
}
