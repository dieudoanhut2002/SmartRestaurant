namespace SmartRestaurant.Domain.Enums;

public enum InvoiceStatus
{
    Pending = 1,        // Chờ thanh toán
    PartiallyPaid = 2,  // Đã thanh toán một phần
    Paid = 3,           // Đã thanh toán đầy đủ
    Cancelled = 4,      // Đã hủy bỏ
    Refunded = 5        // Đã hoàn tiền
}
