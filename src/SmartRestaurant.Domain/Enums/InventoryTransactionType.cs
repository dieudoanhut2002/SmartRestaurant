namespace SmartRestaurant.Domain.Enums;

public enum InventoryTransactionType
{
    Receipt = 1,              // Nhập kho hàng mới
    Adjustment = 2,           // Kiểm kê cân chỉnh kho
    Reservation = 3,          // Báo bếp khóa tạm giữ (Two-Phase Lock D2)
    ReservationCancelled = 4, // Hủy đơn giải phóng tạm giữ
    Consumption = 5,          // Xuất kho chế biến / phục vụ thành công
    Waste = 6                 // Ghi nhận tổn thất / hỏng hóc (WasteLog)
}
