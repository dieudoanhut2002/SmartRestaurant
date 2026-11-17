namespace SmartRestaurant.Domain.Enums;

public enum PaymentMethod
{
    Cash = 1,          // Tiền mặt
    PosCard = 2,       // Thẻ ngân hàng qua POS
    VietQR = 3,        // Dynamic VietQR / Napas 247
    BankTransfer = 4,  // Chuyển khoản trực tiếp
    Mixed = 5          // Hỗn hợp (kết hợp nhiều hình thức)
}
