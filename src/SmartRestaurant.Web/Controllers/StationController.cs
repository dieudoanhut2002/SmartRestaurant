namespace SmartRestaurant.Web.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartRestaurant.Application.Common.Interfaces;
using SmartRestaurant.Application.DTOs;
using SmartRestaurant.Application.Interfaces;
using SmartRestaurant.Domain.Enums;

public class StationController(
    IKitchenStationService stationService,
    IMenuService menuService,
    IAppDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var branch = await dbContext.Branches.FirstOrDefaultAsync(ct);
        var branchId = branch?.Id ?? Guid.Empty;

        var stations = await stationService.GetStationsAsync(branchId, ct);
        var menuItems = await menuService.GetMenuItemsAsync(branchId, ct: ct);

        ViewBag.BranchId = branchId;
        ViewBag.MenuItems = menuItems;

        return View(stations);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateKitchenStationRequest request, CancellationToken ct)
    {
        try
        {
            await stationService.CreateStationAsync(request, ct);
            TempData["SuccessMessage"] = $"Đã tạo trạm bếp '{request.Name}' thành công.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(Guid id, UpdateKitchenStationRequest request, CancellationToken ct)
    {
        try
        {
            await stationService.UpdateStationAsync(id, request, ct);
            TempData["SuccessMessage"] = "Đã cập nhật thông tin trạm bếp thành công.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        try
        {
            await stationService.DeleteStationAsync(id, ct);
            TempData["SuccessMessage"] = "Đã xóa trạm bếp thành công.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> AssignDish([FromBody] AssignDishToStationRequest request, CancellationToken ct)
    {
        try
        {
            var success = await stationService.AssignDishToStationAsync(request, ct);
            return Json(new { success });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
}
