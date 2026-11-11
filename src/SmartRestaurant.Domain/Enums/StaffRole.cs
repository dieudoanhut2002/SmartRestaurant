namespace SmartRestaurant.Domain.Enums;

public enum StaffRole
{
    SystemAdmin = 1,       // Quản Trị Hệ Thống (ADMIN)
    RestaurantManager = 2, // Quản Lý Nhà Hàng (MGR)
    Cashier = 3,           // Thu Ngân (CASH)
    Waiter = 4,            // Bồi Bàn / Nhân Viên Phục Vụ (WAIT)
    Chef = 5,              // Đầu Bếp / Bếp Trưởng (CHEF)
    InventoryStaff = 6     // Thủ Kho (INV)
}
