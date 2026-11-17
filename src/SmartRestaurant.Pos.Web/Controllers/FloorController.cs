namespace SmartRestaurant.Pos.Web.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartRestaurant.Application.Common.Interfaces;
using SmartRestaurant.Application.DTOs;
using SmartRestaurant.Application.Interfaces;

public class FloorController(
    IPosOrderService posOrderService,
    IFloorPlanService floorPlanService,
    IAppDbContext dbContext) : Controller
{
    private readonly IPosOrderService _posOrderService = posOrderService;
    private readonly IFloorPlanService _floorPlanService = floorPlanService;
    private readonly IAppDbContext _dbContext = dbContext;

    public async Task<IActionResult> Index([FromQuery] Guid? areaId)
    {
        var branch = await _dbContext.Branches.FirstOrDefaultAsync();
        var branchId = branch?.Id ?? Guid.Empty;

        var areas = await _floorPlanService.GetDiningAreasAsync(branchId);
        var selectedAreaId = areaId ?? areas.FirstOrDefault()?.Id;

        var tableCards = await _posOrderService.GetTablesQuickViewAsync(branchId, selectedAreaId);

        ViewBag.Areas = areas;
        ViewBag.SelectedAreaId = selectedAreaId;
        ViewBag.BranchId = branchId;

        return View(tableCards);
    }

    [HttpPost]
    public async Task<IActionResult> OpenSession([FromBody] OpenDiningSessionRequest request)
    {
        try
        {
            var result = await _posOrderService.OpenDiningSessionAsync(request);
            return Ok(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> TransferTable([FromBody] TransferTableRequest request)
    {
        try
        {
            var result = await _posOrderService.TransferTableAsync(request);
            return Ok(new { success = true, message = "Chuyển bàn thành công!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> MergeTables([FromBody] MergeTablesRequest request)
    {
        try
        {
            var result = await _posOrderService.MergeTablesAsync(request);
            return Ok(new { success = true, message = "Gộp bàn thành công!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> RequestBill([FromBody] RequestBillRequest request)
    {
        try
        {
            var result = await _posOrderService.RequestBillAsync(request);
            return Ok(new { success = true, message = "Đã gửi yêu cầu tạm tính / in phiếu!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}
