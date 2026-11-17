namespace SmartRestaurant.Pos.Web.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartRestaurant.Application.Common.Interfaces;
using SmartRestaurant.Application.DTOs;
using SmartRestaurant.Application.Interfaces;

public class OrderController(
    IPosOrderService posOrderService,
    IAppDbContext dbContext,
    ILogger<OrderController> logger) : Controller
{
    private readonly IPosOrderService _posOrderService = posOrderService;
    private readonly IAppDbContext _dbContext = dbContext;
    private readonly ILogger<OrderController> _logger = logger;

    [HttpGet("Order/{tableId:guid}")]
    public async Task<IActionResult> Index(Guid tableId)
    {
        var sessionDetail = await _posOrderService.GetTableOrderDetailAsync(tableId);
        if (sessionDetail == null)
        {
            return RedirectToAction("Index", "Floor");
        }

        var branch = await _dbContext.Branches.FirstOrDefaultAsync();
        var branchId = branch?.Id ?? Guid.Empty;

        var catalog = await _posOrderService.GetMenuCatalogAsync(branchId);

        ViewBag.TableId = tableId;
        ViewBag.Catalog = catalog;
        ViewBag.BranchId = branchId;

        return View(sessionDetail);
    }

    [HttpPost]
    public async Task<IActionResult> Fire([FromBody] FireOrderRequest? request)
    {
        if (request == null)
        {
            return BadRequest(new { success = false, message = "Dữ liệu yêu cầu báo bếp không hợp lệ hoặc rỗng." });
        }

        try
        {
            var round = await _posOrderService.FireOrderAsync(request);
            var updatedDetail = await _posOrderService.GetTableOrderDetailAsync(request.TableId);

            return Ok(new
            {
                success = true,
                message = $"Đã báo bếp thành công đợt {round.RoundNumber}!",
                round = round,
                detail = updatedDetail
            });
        }
        catch (SmartRestaurant.Domain.Exceptions.DomainValidationException ex)
        {
            _logger.LogWarning("Báo bếp không hợp lệ bàn {TableId}: {Message}", request.TableId, ex.Message);
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (SmartRestaurant.Domain.Exceptions.DomainNotFoundException ex)
        {
            _logger.LogWarning("Không tìm thấy dữ liệu khi báo bếp bàn {TableId}: {Message}", request.TableId, ex.Message);
            return NotFound(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi máy chủ khi xử lý báo bếp bàn {TableId}: {Message}", request.TableId, ex.Message);
            return StatusCode(500, new { success = false, message = $"Lỗi hệ thống khi báo bếp: {ex.Message}" });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Detail(Guid tableId)
    {
        var sessionDetail = await _posOrderService.GetTableOrderDetailAsync(tableId);
        if (sessionDetail == null)
            return NotFound(new { success = false, message = "Không có phiên bàn hoạt động." });

        return Ok(new { success = true, data = sessionDetail });
    }
}
