namespace SmartRestaurant.Web.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartRestaurant.Application.Common.Interfaces;
using SmartRestaurant.Application.DTOs;
using SmartRestaurant.Application.Interfaces;
using SmartRestaurant.Domain.Enums;

public class FloorPlanController(
    IFloorPlanService floorPlanService,
    IAppDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index(Guid? areaId, CancellationToken ct)
    {
        var branch = await dbContext.Branches.FirstOrDefaultAsync(ct);
        if (branch == null)
        {
            return View(new FloorPlanLayoutDto(Guid.Empty, "Chưa có khu vực", [], []));
        }

        var branchId = branch.Id;
        var layout = await floorPlanService.GetFloorPlanLayoutAsync(branchId, areaId, ct);
        var rooms = await floorPlanService.GetRoomsByAreaAsync(layout.CurrentAreaId, ct);

        ViewBag.BranchId = branchId;
        ViewBag.Rooms = rooms;

        return View(layout);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateRoom(CreateDiningRoomRequest request, CancellationToken ct)
    {
        try
        {
            await floorPlanService.CreateDiningRoomAsync(request, ct);
            TempData["SuccessMessage"] = $"Đã tạo phòng '{request.Name}' thành công.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index), new { areaId = request.DiningAreaId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateRoom(UpdateDiningRoomRequest request, Guid areaId, CancellationToken ct)
    {
        try
        {
            await floorPlanService.UpdateDiningRoomAsync(request, ct);
            TempData["SuccessMessage"] = "Đã cập nhật thông tin phòng thành công.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index), new { areaId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteRoom(Guid roomId, Guid areaId, CancellationToken ct)
    {
        try
        {
            await floorPlanService.DeleteDiningRoomAsync(roomId, ct);
            TempData["SuccessMessage"] = "Đã xóa phòng thành công.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index), new { areaId });
    }

    [HttpPost]
    public async Task<IActionResult> UpdatePosition([FromBody] UpdateTablePositionRequest request, CancellationToken ct)
    {
        var success = await floorPlanService.UpdateTablePositionAsync(request, ct);
        return Json(new { success });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateStatus([FromBody] UpdateTableStatusRequest request, CancellationToken ct)
    {
        var success = await floorPlanService.UpdateTableStatusAsync(request.TableId, request.Status, ct);
        return Json(new { success });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateTable(CreateTableRequest request, CancellationToken ct)
    {
        try
        {
            await floorPlanService.CreateTableAsync(request, ct);
            TempData["SuccessMessage"] = $"Đã tạo bàn '{request.Code}' thành công.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index), new { areaId = request.DiningAreaId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateTable(UpdateTableRequest request, CancellationToken ct)
    {
        try
        {
            await floorPlanService.UpdateTableAsync(request, ct);
            TempData["SuccessMessage"] = "Đã cập nhật thông tin bàn thành công.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index), new { areaId = request.DiningAreaId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteTable(Guid tableId, Guid areaId, CancellationToken ct)
    {
        try
        {
            await floorPlanService.DeleteTableAsync(tableId, ct);
            TempData["SuccessMessage"] = "Đã xóa bàn thành công.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index), new { areaId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateArea(CreateDiningAreaRequest request, CancellationToken ct)
    {
        try
        {
            var area = await floorPlanService.CreateDiningAreaAsync(request, ct);
            TempData["SuccessMessage"] = $"Đã tạo khu vực '{request.Name}' thành công.";
            return RedirectToAction(nameof(Index), new { areaId = area.Id });
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }
}

public record UpdateTableStatusRequest(Guid TableId, TableStatus Status);
