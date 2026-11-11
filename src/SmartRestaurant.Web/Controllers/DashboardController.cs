namespace SmartRestaurant.Web.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartRestaurant.Application.Common.Interfaces;
using SmartRestaurant.Application.Interfaces;

public class DashboardController(
    IDashboardService dashboardService,
    IAppDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var branch = await dbContext.Branches.FirstOrDefaultAsync(ct);
        var branchId = branch?.Id ?? Guid.Empty;

        var stats = await dashboardService.GetManagerDashboardStatsAsync(branchId, ct);
        ViewBag.BranchName = branch?.Name ?? "Nhà Hàng SmartRestaurant";
        ViewBag.BranchId = branchId;

        return View(stats);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }
}
