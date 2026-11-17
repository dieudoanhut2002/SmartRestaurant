namespace SmartRestaurant.Domain.Enums;

public enum SuspenseLedgerStatus
{
    Unreconciled = 1,   // Treo / Chưa đối soát
    Resolved = 2,       // Đã xử lý / Đã gán vào hóa đơn
    Refunded = 3        // Đã hoàn tiền cho khách
}
