namespace SmartRestaurant.Application.Tests;

using System.Text;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SmartRestaurant.Application.Services;
using SmartRestaurant.Domain.Entities;
using SmartRestaurant.Domain.Enums;
using SmartRestaurant.Infrastructure.Data;
using Xunit;

public class ReportServiceAnalyticsTests
{
    private readonly Guid _branchId = Guid.NewGuid();

    private (AppDbContext db, ReportService service, Branch branch, RestaurantTable table, MenuItem steakItem, MenuItem saladItem) CreateTestFixture(string dbName)
    {
        var db = TestDbContextFactory.Create(dbName);
        var service = new ReportService(db);

        var branch = new Branch("Chi nhánh Analytics Test", "TP. Hồ Chí Minh", "0908888888", TimeSpan.FromHours(6));
        db.Branches.Add(branch);

        var area = new DiningArea(branch.Id, "Tầng 1 - Sân Vườn", AreaType.Outdoor, 1);
        db.DiningAreas.Add(area);

        var table = new RestaurantTable(branch.Id, area.Id, "SV-01", "Bàn Sân Vườn 1", 4);
        table.Occupy();
        db.RestaurantTables.Add(table);

        var station = new KitchenStation(branch.Id, "BEP-01", "Bếp Trung Tâm", PrepStation.HotKitchen);
        db.KitchenStations.Add(station);

        var catMains = new MenuCategory(branch.Id, "Món Chính", "Đặc sản nướng", 1);
        var catAppetizers = new MenuCategory(branch.Id, "Khai Vị", "Salad & súp", 2);
        db.MenuCategories.AddRange(catMains, catAppetizers);

        var steakItem = new MenuItem(branch.Id, catMains.Id, "STEAK-01", "Bò Wagyu Sốt Nấm", 350000m);
        var saladItem = new MenuItem(branch.Id, catAppetizers.Id, "SALAD-01", "Salad Cá Hồi", 120000m);
        db.MenuItems.AddRange(steakItem, saladItem);

        // Dining Session & Completed Order
        var session = new DiningSession(branch.Id, table.Id, "SS-ANALYTICS-01", 3);
        db.DiningSessions.Add(session);

        var order = new Order(branch.Id, session.Id, "OD-ANALYTICS-01", DateTime.UtcNow);
        var staffId = Guid.NewGuid();
        var round = new OrderRound(order.Id, 1, staffId, Guid.NewGuid().ToString());

        var item1 = new OrderItem(round.Id, steakItem.Id, steakItem.Name, steakItem.BasePrice, 30); // 30 portions
        var item2 = new OrderItem(round.Id, saladItem.Id, saladItem.Name, saladItem.BasePrice, 5);  // 5 portions
        round.AddItem(item1);
        round.AddItem(item2);

        order.AddRound(round);
        order.Settle();
        db.Orders.Add(order);
        db.OrderRounds.Add(round);
        db.OrderItems.AddRange(item1, item2);

        session.AddAmount(order.TotalAmount);
        session.Complete();

        // Kitchen Tickets
        var ticketFast = new KitchenTicket(station.Id, round.Id, "TK-01", "[]", null, table.Code);
        ticketFast.StartPreparation();
        ticketFast.Complete(); // completed quickly

        var ticketOverdue = new KitchenTicket(station.Id, round.Id, "TK-02", "[]", null, table.Code);
        ticketOverdue.StartPreparation();

        db.KitchenTickets.AddRange(ticketFast, ticketOverdue);

        db.SaveChanges();

        return (db, service, branch, table, steakItem, saladItem);
    }

    [Fact]
    public async Task GetExecutiveBiDashboardAsync_ShouldReturnValidMetrics_WithRealData()
    {
        var (db, service, branch, table, steakItem, saladItem) = CreateTestFixture(Guid.NewGuid().ToString());

        var dashboard = await service.GetExecutiveBiDashboardAsync(
            branch.Id,
            DateTime.UtcNow.AddDays(-1),
            DateTime.UtcNow.AddDays(1));

        dashboard.Should().NotBeNull();
        dashboard.HourlyHeatmap.Should().HaveCount(24);
        dashboard.HourlyHeatmap[0].Hour.Should().Be(0);
        dashboard.HourlyHeatmap[23].Hour.Should().Be(23);

        // Gross margin check
        dashboard.TotalRevenue.Should().BeGreaterThan(0);
        dashboard.TotalGrossProfit.Should().BeGreaterThan(0);
        dashboard.OverallMarginPercent.Should().BeGreaterThan(0);

        // KDS SLA metrics check
        dashboard.SlaPerformance.Should().NotBeNull();
        dashboard.SlaPerformance.TotalTickets.Should().BeGreaterThan(0);

        // Menu engineering matrix check
        dashboard.MenuEngineering.Should().NotBeEmpty();
        dashboard.MenuEngineering.Should().Contain(m => m.DishCode == "STEAK-01");
        dashboard.MenuEngineering.Should().Contain(m => m.DishCode == "SALAD-01");

        // Top tables turnover check
        dashboard.TopTables.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetExecutiveBiDashboardAsync_Fallback_WhenNoDataExists()
    {
        var db = TestDbContextFactory.Create(Guid.NewGuid().ToString());
        var service = new ReportService(db);
        var branch = new Branch("Chi nhánh Mới Khai Trương", "Đà Nẵng", "0901112222", TimeSpan.FromHours(6));
        db.Branches.Add(branch);
        await db.SaveChangesAsync();

        var dashboard = await service.GetExecutiveBiDashboardAsync(
            branch.Id,
            DateTime.UtcNow.AddYears(-5),
            DateTime.UtcNow.AddYears(-4));

        dashboard.Should().NotBeNull();
        dashboard.TotalRevenue.Should().BeGreaterThan(0, "Realistic preview fallback should supply illustrative figures");
        dashboard.HourlyHeatmap.Should().HaveCount(24);
        dashboard.HourlyHeatmap.Should().Contain(h => h.PeakLevel == "Cao điểm" || h.PeakLevel == "Bình thường");
        dashboard.MenuEngineering.Should().NotBeEmpty();
        dashboard.TopTables.Should().NotBeEmpty();
        dashboard.SlaPerformance.TotalTickets.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task ExportSalesReportCsvAsync_ProducesValidUtf8WithBom()
    {
        var (db, service, branch, _, _, _) = CreateTestFixture(Guid.NewGuid().ToString());

        var csvBytes = await service.ExportSalesReportCsvAsync(branch.Id, DateTime.UtcNow.Date);

        csvBytes.Should().NotBeNull();
        csvBytes.Length.Should().BeGreaterThan(3);

        // Check UTF-8 BOM: 0xEF, 0xBB, 0xBF
        csvBytes[0].Should().Be(0xEF);
        csvBytes[1].Should().Be(0xBB);
        csvBytes[2].Should().Be(0xBF);

        var content = Encoding.UTF8.GetString(csvBytes);
        content.Should().Contain("SMARTRESTAURANT — BÁO CÁO DOANH THU HÀNG NGÀY");
        content.Should().Contain("DOANH THU THEO DANH MỤC THỰC ĐƠN");
        content.Should().Contain("Tên Danh Mục,Số Lượng Món Bán,Doanh Thu (VNĐ),Tỷ Trọng (%)");
    }

    [Fact]
    public async Task ExportMenuEngineeringCsvAsync_ProducesValidUtf8WithBom()
    {
        var (db, service, branch, _, _, _) = CreateTestFixture(Guid.NewGuid().ToString());

        var csvBytes = await service.ExportMenuEngineeringCsvAsync(branch.Id);

        csvBytes.Should().NotBeNull();
        csvBytes.Length.Should().BeGreaterThan(3);

        // Check UTF-8 BOM
        csvBytes[0].Should().Be(0xEF);
        csvBytes[1].Should().Be(0xBB);
        csvBytes[2].Should().Be(0xBF);

        var content = Encoding.UTF8.GetString(csvBytes);
        content.Should().Contain("SMARTRESTAURANT — MA TRẬN PHÂN TÍCH HIỆU QUẢ THỰC ĐƠN");
        content.Should().Contain("Mã Món");
        content.Should().Contain("Tên Món Ăn");
        content.Should().Contain("Đơn Giá Bán (VNĐ)");
        content.Should().Contain("Phân Loại BCG");
    }

    [Fact]
    public async Task GetExecutiveBiDashboardAsync_ShouldAutoResolveBranchAndFetchSessions_WhenDatesAndBranchAreDefault()
    {
        var (db, service, branch, table, steakItem, saladItem) = CreateTestFixture(Guid.NewGuid().ToString());

        // Call with Guid.Empty and null dates (mimicking default user visit to /Report/Analytics)
        var dashboard = await service.GetExecutiveBiDashboardAsync(Guid.Empty, null, null);

        dashboard.Should().NotBeNull();
        dashboard.TotalRevenue.Should().BeGreaterThan(0);
        dashboard.TotalOrders.Should().BeGreaterThan(0);
        dashboard.TopTables.Should().Contain(t => t.TableCode == table.Code);
        dashboard.MenuEngineering.Should().Contain(m => m.DishCode == steakItem.Code && m.QuantitySold == 30);
    }
}
