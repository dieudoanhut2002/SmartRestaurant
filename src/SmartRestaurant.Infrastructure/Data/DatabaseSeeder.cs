namespace SmartRestaurant.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;
using SmartRestaurant.Domain.Entities;
using SmartRestaurant.Domain.Enums;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        await context.Database.EnsureCreatedAsync();

        // Check if already seeded
        if (await context.Branches.AnyAsync())
            return;

        // 1. Seed Branch
        var branch = new Branch(
            "SmartRestaurant — Chi Nhánh 1 (Trung Tâm)",
            "123 Đường Nguyễn Huệ, Quận 1, TP. Hồ Chí Minh",
            "028 3822 9999",
            new TimeSpan(6, 0, 0)); // 06:00 BusinessDate Cutoff per D1

        context.Branches.Add(branch);
        await context.SaveChangesAsync();

        var branchId = branch.Id;

        // 2. Seed Staff Accounts (PINs per specs)
        var staffList = new List<StaffMember>
        {
            new(branchId, "Nguyễn Văn An (Quản Trị)", "admin", "admin@smartrestaurant.vn", StaffRole.SystemAdmin, "9999"),
            new(branchId, "Trần Thị Bình (Quản Lý)", "manager", "manager@smartrestaurant.vn", StaffRole.RestaurantManager, "1234"),
            new(branchId, "Lê Hoàng Cường (Thu Ngân)", "cashier", "cashier@smartrestaurant.vn", StaffRole.Cashier, "1111"),
            new(branchId, "Phạm Minh Dũng (Bồi Bàn)", "waiter", "waiter@smartrestaurant.vn", StaffRole.Waiter, "2222"),
            new(branchId, "Võ Quốc Hùng (Bếp Trưởng)", "chef", "chef@smartrestaurant.vn", StaffRole.Chef, "3333"),
            new(branchId, "Đặng Thu Thảo (Thủ Kho)", "inventory", "inventory@smartrestaurant.vn", StaffRole.InventoryStaff, "4444")
        };

        context.StaffMembers.AddRange(staffList);
        await context.SaveChangesAsync();

        // 3. Seed Dining Areas & Rooms
        var outdoorArea = new DiningArea(branchId, "Tầng 1 — Sân Vườn", AreaType.Outdoor, 1);
        var indoorArea = new DiningArea(branchId, "Tầng 2 — Phòng Lạnh", AreaType.Indoor, 2);
        var vipArea = new DiningArea(branchId, "Tầng 3 — Phòng VIP", AreaType.VIP, 3);

        context.DiningAreas.AddRange(outdoorArea, indoorArea, vipArea);
        await context.SaveChangesAsync();

        var vipRoom1 = new DiningRoom(vipArea.Id, "Phòng VIP Hoàng Gia", isVip: true, surchargeRate: 10m, surchargeFixedAmount: 200000m);
        var vipRoom2 = new DiningRoom(vipArea.Id, "Phòng VIP Sen Vàng", isVip: true, surchargeRate: 10m, surchargeFixedAmount: 150000m);
        context.DiningRooms.AddRange(vipRoom1, vipRoom2);
        await context.SaveChangesAsync();

        // 4. Seed Tables with Visual Konva.js Floor Coordinates
        var tables = new List<RestaurantTable>
        {
            // Tầng 1 — Sân Vườn (Outdoor)
            new(branchId, outdoorArea.Id, "SV-01", "Bàn Sân Vườn 01", 4, shape: TableShape.Square, posX: 60, posY: 60, width: 90, height: 90),
            new(branchId, outdoorArea.Id, "SV-02", "Bàn Sân Vườn 02", 4, shape: TableShape.Square, posX: 200, posY: 60, width: 90, height: 90),
            new(branchId, outdoorArea.Id, "SV-03", "Bàn Sân Vườn 03", 6, shape: TableShape.Rectangle, posX: 340, posY: 60, width: 140, height: 90),
            new(branchId, outdoorArea.Id, "SV-04", "Bàn Sân Vườn 04", 2, shape: TableShape.Round, posX: 60, posY: 200, width: 90, height: 90),
            new(branchId, outdoorArea.Id, "SV-05", "Bàn Sân Vườn 05", 2, shape: TableShape.Round, posX: 200, posY: 200, width: 90, height: 90),
            new(branchId, outdoorArea.Id, "SV-06", "Bàn Sân Vườn 06", 8, shape: TableShape.Rectangle, posX: 340, posY: 200, width: 180, height: 90),

            // Tầng 2 — Phòng Lạnh (Indoor)
            new(branchId, indoorArea.Id, "PL-01", "Bàn Lạnh 01", 4, shape: TableShape.Rectangle, posX: 60, posY: 60, width: 100, height: 80),
            new(branchId, indoorArea.Id, "PL-02", "Bàn Lạnh 02", 4, shape: TableShape.Rectangle, posX: 200, posY: 60, width: 100, height: 80),
            new(branchId, indoorArea.Id, "PL-03", "Bàn Lạnh 03", 4, shape: TableShape.Rectangle, posX: 340, posY: 60, width: 100, height: 80),
            new(branchId, indoorArea.Id, "PL-04", "Bàn Lạnh 04", 6, shape: TableShape.Rectangle, posX: 480, posY: 60, width: 130, height: 80),
            new(branchId, indoorArea.Id, "PL-05", "Bàn Lạnh 05", 2, shape: TableShape.Square, posX: 60, posY: 200, width: 80, height: 80),
            new(branchId, indoorArea.Id, "PL-06", "Bàn Lạnh 06", 2, shape: TableShape.Square, posX: 200, posY: 200, width: 80, height: 80),
            new(branchId, indoorArea.Id, "PL-07", "Bàn Lạnh 07", 10, shape: TableShape.Rectangle, posX: 340, posY: 200, width: 220, height: 90),

            // Tầng 3 — Phòng VIP
            new(branchId, vipArea.Id, "VIP-01", "Bàn Hoàng Gia 1", 8, vipRoom1.Id, TableShape.Round, posX: 100, posY: 100, width: 140, height: 140),
            new(branchId, vipArea.Id, "VIP-02", "Bàn Hoàng Gia 2", 6, vipRoom1.Id, TableShape.Rectangle, posX: 300, posY: 100, width: 160, height: 90),
            new(branchId, vipArea.Id, "VIP-03", "Bàn Sen Vàng 1", 10, vipRoom2.Id, TableShape.Round, posX: 150, posY: 280, width: 160, height: 160)
        };

        // Set some realistic active states
        tables[0].Occupy(); // SV-01 Occupied
        tables[1].Reserve(); // SV-02 Reserved
        tables[6].Occupy(); // PL-01 Occupied
        tables[6].RequestBilling(); // PL-01 BillingRequested
        tables[7].Occupy(); // PL-02 Occupied
        tables[8].Occupy();
        tables[8].ReleaseToCleaning(); // PL-03 CleaningNeeded

        context.RestaurantTables.AddRange(tables);
        await context.SaveChangesAsync();

        // 5. Seed Kitchen Stations per Requirements (Unified KDS with multiple stations)
        var stationHot = new KitchenStation(branchId, "KS-HOT", "Bếp Nóng", PrepStation.HotKitchen, "Khu chế biến món chiên, xào, kho, lẩu, nướng", 1, "192.168.1.201");
        var stationCold = new KitchenStation(branchId, "KS-COLD", "Bếp Nguội & Salad", PrepStation.ColdKitchen, "Khu chế biến gỏi, salad và món khai vị nguội", 2, "192.168.1.202");
        var stationBar = new KitchenStation(branchId, "KS-BAR", "Quầy Bar & Pha Chế", PrepStation.Bar, "Khu pha chế đồ uống, trà hoa quả và xuất bia ướp lạnh", 3, "192.168.1.203");
        var stationBake = new KitchenStation(branchId, "KS-BAKE", "Bếp Bánh & Tráng Miệng", PrepStation.Bakery, "Khu chuẩn bị chè, kem và bánh ngọt tráng miệng", 4, "192.168.1.204");

        context.KitchenStations.AddRange(stationHot, stationCold, stationBar, stationBake);
        await context.SaveChangesAsync();

        // 6. Seed Menu Categories
        var catKhaiVi = new MenuCategory(branchId, "Khai Vị", "Các món khai vị kích thích vị giác", 1);
        var catMonChinh = new MenuCategory(branchId, "Món Chính", "Món ăn chính đặc sắc phong vị Việt", 2);
        var catLauNuong = new MenuCategory(branchId, "Lẩu & Nướng", "Các món nướng than hoa và lẩu niêu", 3);
        var catDoUong = new MenuCategory(branchId, "Đồ Uống & Bar", "Bia, nước ngọt đóng chai và cocktail pha chế", 4);
        var catTrangMieng = new MenuCategory(branchId, "Tráng Miệng", "Chè cung đình, kem và bánh ngọt", 5);

        context.MenuCategories.AddRange(catKhaiVi, catMonChinh, catLauNuong, catDoUong, catTrangMieng);
        await context.SaveChangesAsync();

        // 7. Seed Rich Menu Items with Prices and Stations
        var menuItems = new List<MenuItem>
        {
            // Khai Vị
            new(branchId, catKhaiVi.Id, "KV-01", "Gỏi Ngó Sen Tôm Thịt", 95000m, "Ngó sen tươi giòn kèm tôm sú và thịt ba chỉ", "/images/menu/goi-ngo-sen.jpg", PrepStation.ColdKitchen),
            new(branchId, catKhaiVi.Id, "KV-02", "Chả Giò Hải Sản Hoàng Gia", 85000m, "Chả giò chiên giòn rụm nhân tôm cua", "/images/menu/cha-gio.jpg", PrepStation.HotKitchen),
            new(branchId, catKhaiVi.Id, "KV-03", "Salad Bò Trộn Giấm Bổng", 110000m, "Thăn bò áp chảo trộn rau mầm sốt giấm bổng", "/images/menu/salad-bo.jpg", PrepStation.ColdKitchen),

            // Món Chính
            new(branchId, catMonChinh.Id, "MC-01", "Bò Lúc Lắc Khoai Tây", 185000m, "Thăn bò Úc xào hành tây, ớt chuông kèm khoai giòn", "/images/menu/bo-luc-lac.jpg", PrepStation.HotKitchen),
            new(branchId, catMonChinh.Id, "MC-02", "Gà Ta Hấp Lá Chanh", 260000m, "Nửa con gà ta thả vườn thịt chắc, da giòn thơm lá chanh", "/images/menu/ga-hap.jpg", PrepStation.HotKitchen),
            new(branchId, catMonChinh.Id, "MC-03", "Cá Chẽm Chiên Sốt Chua Ngọt", 240000m, "Cá chẽm tươi phi lê chiên vàng ruộm sốt thơm", "/images/menu/ca-chem.jpg", PrepStation.HotKitchen),
            new(branchId, catMonChinh.Id, "MC-04", "Cơm Chiên Hải Sản Hạt Sen", 125000m, "Cơm đảo vàng óng tôm mực tươi và hạt sen bùi", "/images/menu/com-chien.jpg", PrepStation.HotKitchen),

            // Lẩu & Nướng
            new(branchId, catLauNuong.Id, "LN-01", "Lẩu Thái Hải Sản Chua Cay", 350000m, "Nước lẩu tôm yum đậm đà kèm đĩa hải sản ngập tràn", "/images/menu/lau-thai.jpg", PrepStation.HotKitchen),
            new(branchId, catLauNuong.Id, "LN-02", "Ba Chỉ Bò Mỹ Nướng Sốt Tiêu Đen", 195000m, "Thịt bò Mỹ nướng than hoa sốt tiêu đen thơm nồng", "/images/menu/ba-chi-nuong.jpg", PrepStation.HotKitchen),
            new(branchId, catLauNuong.Id, "LN-03", "Lẩu Nấm Chim Câu Bổ Dưỡng", 380000m, "Nước dùng thanh ngọt từ xương hầm và các loại nấm quý", "/images/menu/lau-nam.jpg", PrepStation.HotKitchen),

            // Đồ Uống & Bar (Một số món DirectFOHPick bỏ qua KDS per REV-08)
            new(branchId, catDoUong.Id, "DU-01", "Bia Heineken Silver (Lon 330ml)", 32000m, "Bia ướp lạnh sẵn, phục vụ trực tiếp tại sảnh", "/images/menu/heineken.jpg", PrepStation.Bar, directFOHPick: true, isCountable: true, inStockQuantity: 120),
            new(branchId, catDoUong.Id, "DU-02", "Bia Sài Gòn Special (Lon 330ml)", 25000m, "Bia ướp lạnh sẵn, phục vụ trực tiếp tại sảnh", "/images/menu/saigon-special.jpg", PrepStation.Bar, directFOHPick: true, isCountable: true, inStockQuantity: 150),
            new(branchId, catDoUong.Id, "DU-03", "Trà Đào Cam Sả Tươi", 45000m, "Trà ủ lạnh kèm đào ngâm giòn thơm nồng sả", "/images/menu/tra-dao.jpg", PrepStation.Bar, directFOHPick: false),
            new(branchId, catDoUong.Id, "DU-04", "Nước Suối Lavie (Chai 500ml)", 15000m, "Nước khoáng thiên nhiên ướp lạnh", "/images/menu/lavie.jpg", PrepStation.Bar, directFOHPick: true, isCountable: true, inStockQuantity: 80),
            new(branchId, catDoUong.Id, "DU-05", "Cocktail Mojito Bạc Hà", 75000m, "Cocktail rum chanh tươi lá bạc hà sảng khoái", "/images/menu/mojito.jpg", PrepStation.Bar, directFOHPick: false),

            // Tráng Miệng
            new(branchId, catTrangMieng.Id, "TM-01", "Chè Hạt Sen Long Nhãn", 40000m, "Chè thanh mát giải nhiệt mùa hè", "/images/menu/che-hat-sen.jpg", PrepStation.Bakery),
            new(branchId, catTrangMieng.Id, "TM-02", "Bánh Flan Caramen Cốt Dừa", 30000m, "Bánh flan mềm mịn thơm ngậy cà phê và cốt dừa", "/images/menu/flan.jpg", PrepStation.Bakery)
        };

        // Link items to actual KitchenStationId
        foreach (var item in menuItems)
        {
            switch (item.PrepStation)
            {
                case PrepStation.HotKitchen:
                    item.AssignToStation(stationHot.Id, PrepStation.HotKitchen);
                    break;
                case PrepStation.ColdKitchen:
                    item.AssignToStation(stationCold.Id, PrepStation.ColdKitchen);
                    break;
                case PrepStation.Bar:
                    item.AssignToStation(stationBar.Id, PrepStation.Bar);
                    break;
                case PrepStation.Bakery:
                    item.AssignToStation(stationBake.Id, PrepStation.Bakery);
                    break;
            }
        }

        context.MenuItems.AddRange(menuItems);
        await context.SaveChangesAsync();

        // 8. Seed Variants & Modifier Groups
        var boLucLac = menuItems.First(m => m.Code == "MC-01");
        context.MenuItemVariants.AddRange(
            new MenuItemVariant(boLucLac.Id, "Khẩu phần Vừa (200g)", 0, "MC01-M"),
            new MenuItemVariant(boLucLac.Id, "Khẩu phần Lớn (350g)", 60000m, "MC01-L"));

        var lauThai = menuItems.First(m => m.Code == "LN-01");
        context.MenuItemVariants.AddRange(
            new MenuItemVariant(lauThai.Id, "Nồi Nhỏ (2-3 khách)", 0, "LN01-S"),
            new MenuItemVariant(lauThai.Id, "Nồi Lớn (4-6 khách)", 100000m, "LN01-L"));

        var groupMucCay = new ModifierGroup(branchId, "Mức Độ Cay", minSelections: 1, maxSelections: 1);
        var groupToppingLau = new ModifierGroup(branchId, "Topping Gọi Thêm", minSelections: 0, maxSelections: 5);
        var groupDuongDa = new ModifierGroup(branchId, "Lựa Chọn Đường & Đá", minSelections: 1, maxSelections: 1);

        context.ModifierGroups.AddRange(groupMucCay, groupToppingLau, groupDuongDa);
        await context.SaveChangesAsync();

        context.ModifierOptions.AddRange(
            new ModifierOption(groupMucCay.Id, "Không Cay (Trẻ Em)", 0m),
            new ModifierOption(groupMucCay.Id, "Cay Vừa (Tiêu Chuẩn)", 0m),
            new ModifierOption(groupMucCay.Id, "Siêu Cay Thái", 0m),

            new ModifierOption(groupToppingLau.Id, "Thịt Ba Chỉ Bò Mỹ (100g)", 55000m),
            new ModifierOption(groupToppingLau.Id, "Nấm Linh Chi & Nấm Kim Châm", 35000m),
            new ModifierOption(groupToppingLau.Id, "Đậu Hũ Non Nhúng Lẩu", 15000m),

            new ModifierOption(groupDuongDa.Id, "100% Đường - 100% Đá (Chuẩn)", 0m),
            new ModifierOption(groupDuongDa.Id, "70% Đường - 50% Đá", 0m),
            new ModifierOption(groupDuongDa.Id, "30% Đường - Ít Đá", 0m),
            new ModifierOption(groupDuongDa.Id, "Không Đá / Uống Nóng", 0m));
        await context.SaveChangesAsync();

        // 9. Seed Inventory Items for Countable Goods (D5, D2, INV-03)
        var heinekenItem = menuItems.First(m => m.Code == "DU-01");
        var saigonItem = menuItems.First(m => m.Code == "DU-02");
        var lavieItem = menuItems.First(m => m.Code == "DU-04");
        var baChiItem = menuItems.First(m => m.Code == "LN-02");
        var caChemItem = menuItems.First(m => m.Code == "MC-03");

        var invHeineken = new InventoryItem(branchId, "INV-DU01", "Bia Heineken Silver (Lon 330ml)", "Bia & Nước Giải Khát", "Lon", 120, 24, heinekenItem.Id);
        invHeineken.ReserveStock(6);
        invHeineken.ConsumeStock(12);
        invHeineken.RecordWaste(1);

        var invSaigon = new InventoryItem(branchId, "INV-DU02", "Bia Sài Gòn Special (Lon 330ml)", "Bia & Nước Giải Khát", "Lon", 150, 30, saigonItem.Id);
        invSaigon.ReserveStock(10);
        invSaigon.ConsumeStock(20);

        var invLavie = new InventoryItem(branchId, "INV-DU04", "Nước Suối Lavie (Chai 500ml)", "Bia & Nước Giải Khát", "Chai", 80, 20, lavieItem.Id);
        invLavie.ReserveStock(4);
        invLavie.ConsumeStock(8);

        var invBaChi = new InventoryItem(branchId, "INV-TP01", "Ba Chỉ Bò Mỹ Đóng Gói (Khay 500g)", "Thịt Bò Nhập Khẩu", "Khay", 40, 10, baChiItem.Id);
        invBaChi.ReserveStock(3);
        invBaChi.ConsumeStock(15);

        var invCaChem = new InventoryItem(branchId, "INV-HS01", "Cá Chẽm Tươi Nguyên Con (1kg)", "Hải Sản Tươi Sống", "Con", 20, 5, caChemItem.Id);
        invCaChem.ReserveStock(2);
        invCaChem.ConsumeStock(5);
        invCaChem.RecordWaste(1);

        context.InventoryItems.AddRange(invHeineken, invSaigon, invLavie, invBaChi, invCaChem);
        await context.SaveChangesAsync();

        // 10. Seed Inventory Transactions
        context.InventoryTransactions.AddRange(
            new InventoryTransaction(branchId, invHeineken.Id, invHeineken.ItemCode, invHeineken.ItemName, InventoryTransactionType.Receipt, 120, 0, 120, "Nhập kho lô bia đầu tuần từ NPP Sabeco/Heineken", "Đặng Thu Thảo"),
            new InventoryTransaction(branchId, invSaigon.Id, invSaigon.ItemCode, invSaigon.ItemName, InventoryTransactionType.Receipt, 150, 0, 150, "Nhập kho lô bia đầu tuần từ NPP Sabeco", "Đặng Thu Thảo"),
            new InventoryTransaction(branchId, invLavie.Id, invLavie.ItemCode, invLavie.ItemName, InventoryTransactionType.Receipt, 80, 0, 80, "Nhập nước suối Lavie thùng 24 chai", "Đặng Thu Thảo"),
            new InventoryTransaction(branchId, invHeineken.Id, invHeineken.ItemCode, invHeineken.ItemName, InventoryTransactionType.Adjustment, 120, 120, 120, "Kiểm kê đầu ca sáng, khớp số lượng thực tế", "Đặng Thu Thảo"));
        await context.SaveChangesAsync();

        // 8. Seed Sample Active Dining Session & Shift for Dashboard Numbers
        var cashier = staffList.First(s => s.Role == StaffRole.Cashier);
        var manager = staffList.First(s => s.Role == StaffRole.RestaurantManager);

        var shift = new WorkShift(branchId, cashier.Id, 2000000m, "Ca sáng ngày làm việc");
        shift.AddSystemCash(6450000m);
        context.WorkShifts.Add(shift);

        var session1 = new DiningSession(branchId, tables[0].Id, "SES-20261111-001", 4);
        session1.AddAmount(840000m);

        var session2 = new DiningSession(branchId, tables[6].Id, "SES-20261111-002", 3);
        session2.AddAmount(1250000m);

        var session3 = new DiningSession(branchId, tables[7].Id, "SES-20261111-003", 2);
        session3.AddAmount(490000m);

        context.DiningSessions.AddRange(session1, session2, session3);

        // 9. Seed Sample Audit Logs
        var audit = new ManagerPinAuditLog(
            branchId,
            manager.Id,
            manager.FullName,
            "Chiết khấu hóa đơn",
            "Bàn PL-01 / Bill #1002",
            "Khách VIP thân thiết giảm 10%");

        var waste = new KitchenWasteLog(
            branchId,
            menuItems[0].Id,
            menuItems[0].Name,
            1,
            "Khách đổi ý sau khi bếp đã xào",
            65000m,
            manager.Id,
            manager.FullName);

        context.ManagerPinAuditLogs.Add(audit);
        context.KitchenWasteLogs.Add(waste);

        await context.SaveChangesAsync();
    }
}
