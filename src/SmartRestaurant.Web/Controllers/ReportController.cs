namespace SmartRestaurant.Web.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartRestaurant.Application.Common.Interfaces;
using SmartRestaurant.Application.DTOs;
using SmartRestaurant.Application.Interfaces;

public class ReportController(
    IReportService reportService,
    IMenuService menuService,
    IAppDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index(DateTime? businessDate, CancellationToken ct)
    {
        var branch = await dbContext.Branches.FirstOrDefaultAsync(ct);
        var branchId = branch?.Id ?? Guid.Empty;

        var report = await reportService.GetDailySalesReportAsync(branchId, businessDate, ct);
        var topDishes = await reportService.GetTopSellingDishesAsync(branchId, 10, ct);

        ViewBag.TopDishes = topDishes;
        ViewBag.SelectedDate = businessDate?.ToString("yyyy-MM-dd") ?? DateTime.UtcNow.ToString("yyyy-MM-dd");
        ViewBag.BranchId = branchId;

        return View(report);
    }

    public async Task<IActionResult> WasteLogs(CancellationToken ct)
    {
        var branch = await dbContext.Branches.FirstOrDefaultAsync(ct);
        var branchId = branch?.Id ?? Guid.Empty;

        var logs = await reportService.GetKitchenWasteLogsAsync(branchId, ct);
        var menuItems = await menuService.GetMenuItemsAsync(branchId, null, null, ct);

        ViewBag.MenuItems = menuItems;
        ViewBag.BranchId = branchId;

        return View(logs);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LogWaste(
        Guid menuItemId,
        int quantity,
        string reason,
        decimal estimatedLoss,
        CancellationToken ct)
    {
        var branch = await dbContext.Branches.FirstOrDefaultAsync(ct);
        var branchId = branch?.Id ?? Guid.Empty;

        try
        {
            await reportService.LogKitchenWasteAsync(
                branchId,
                menuItemId,
                quantity,
                reason,
                estimatedLoss,
                null,
                "Quản Lý Nhà Hàng",
                ct);

            TempData["SuccessMessage"] = "Đã ghi nhận tổn thất hao phí bếp vào WasteLog.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(WasteLogs));
    }

    public async Task<IActionResult> PinAudits(CancellationToken ct)
    {
        var branch = await dbContext.Branches.FirstOrDefaultAsync(ct);
        var branchId = branch?.Id ?? Guid.Empty;

        var audits = await reportService.GetManagerPinAuditLogsAsync(branchId, ct);
        return View(audits);
    }

    public async Task<IActionResult> Analytics(DateTime? fromDate, DateTime? toDate, CancellationToken ct)
    {
        var branch = await dbContext.Branches.FirstOrDefaultAsync(ct);
        var branchId = branch?.Id ?? Guid.Empty;

        ExecutiveBiDashboardDto dashboard = await reportService.GetExecutiveBiDashboardAsync(branchId, fromDate, toDate, ct);
        ViewBag.BranchId = branchId;
        ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
        ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");

        return View(dashboard);
    }

    public async Task<IActionResult> ExportDailySalesCsv(DateTime? businessDate, CancellationToken ct)
    {
        var branch = await dbContext.Branches.FirstOrDefaultAsync(ct);
        var branchId = branch?.Id ?? Guid.Empty;

        var csvBytes = await reportService.ExportSalesReportCsvAsync(branchId, businessDate, ct);
        var dateStr = (businessDate ?? DateTime.UtcNow).ToString("yyyyMMdd");
        var fileName = $"BaoCaoDoanhThu_{dateStr}.csv";

        return File(csvBytes, "text/csv; charset=utf-8", fileName);
    }

    public async Task<IActionResult> ExportMenuEngineeringCsv(CancellationToken ct)
    {
        var branch = await dbContext.Branches.FirstOrDefaultAsync(ct);
        var branchId = branch?.Id ?? Guid.Empty;

        var csvBytes = await reportService.ExportMenuEngineeringCsvAsync(branchId, ct);
        var dateStr = DateTime.UtcNow.ToString("yyyyMMdd");
        var fileName = $"PhanTichThucDon_BCG_{dateStr}.csv";

        return File(csvBytes, "text/csv; charset=utf-8", fileName);
    }
}
