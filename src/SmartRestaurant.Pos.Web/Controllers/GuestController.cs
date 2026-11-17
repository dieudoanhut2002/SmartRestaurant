namespace SmartRestaurant.Pos.Web.Controllers;

using Microsoft.AspNetCore.Mvc;
using SmartRestaurant.Application.DTOs;
using SmartRestaurant.Application.Interfaces;
using SmartRestaurant.Domain.Exceptions;

public class GuestOrderViewModel
{
    public GuestTableSessionDto Session { get; set; } = null!;
    public IReadOnlyList<MenuCategoryDto> Categories { get; set; } = [];
    public IReadOnlyList<MenuItemDto> MenuItems { get; set; } = [];
    public string Token { get; set; } = string.Empty;
}

public class GuestController(
    IGuestOrderService guestOrderService,
    IMenuService menuService,
    IFloorPlanService floorPlanService,
    ILogger<GuestController> logger) : Controller
{
    private readonly IGuestOrderService _guestOrderService = guestOrderService;
    private readonly IMenuService _menuService = menuService;
    private readonly IFloorPlanService _floorPlanService = floorPlanService;
    private readonly ILogger<GuestController> _logger = logger;

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] Guid tableId, [FromQuery] string? token, CancellationToken ct)
    {
        if (tableId == Guid.Empty || string.IsNullOrWhiteSpace(token))
        {
            ViewBag.ErrorMessage = "Thiếu thông tin bàn hoặc mã xác thực bảo mật QR.";
            return View("InvalidQr");
        }

        try
        {
            var session = await _guestOrderService.GetTableSessionAsync(tableId, token, ct);
            var table = await _floorPlanService.GetTableByIdAsync(tableId, ct)
                ?? throw new DomainNotFoundException("Không tìm thấy bàn yêu cầu.");
            var branchId = table.BranchId;

            var categories = await _menuService.GetCategoriesAsync(branchId, ct);
            var items = await _menuService.GetMenuItemsAsync(branchId, ct: ct);

            var vm = new GuestOrderViewModel
            {
                Session = session,
                Categories = categories,
                MenuItems = items,
                Token = token
            };

            return View(vm);
        }
        catch (DomainValidationException ex)
        {
            ViewBag.ErrorMessage = ex.Message;
            return View("InvalidQr");
        }
        catch (DomainNotFoundException ex)
        {
            ViewBag.ErrorMessage = ex.Message;
            return View("InvalidQr");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi xảy ra khi thực khách truy cập thực đơn bàn {TableId}", tableId);
            ViewBag.ErrorMessage = "Có lỗi xảy ra khi tải dữ liệu bàn ăn. Vui lòng quét lại mã QR tại bàn.";
            return View("InvalidQr");
        }
    }

    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> SubmitOrder([FromBody] GuestOrderRequestDto request, CancellationToken ct)
    {
        if (request == null)
        {
            return BadRequest(new { success = false, message = "Dữ liệu yêu cầu không hợp lệ." });
        }

        try
        {
            var result = await _guestOrderService.SubmitGuestOrderAsync(request, ct);
            return Ok(new { success = true, data = result });
        }
        catch (DomainValidationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (DomainNotFoundException ex)
        {
            return NotFound(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi thực khách tự gửi gọi món bàn {TableId}", request.TableId);
            return StatusCode(500, new { success = false, message = "Lỗi hệ thống khi gửi món ăn. Quý khách vui lòng gọi phục vụ để được hỗ trợ." });
        }
    }

    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> RequestBill([FromBody] GuestRequestBillDto request, CancellationToken ct)
    {
        if (request == null || request.TableId == Guid.Empty)
        {
            return BadRequest(new { success = false, message = "Dữ liệu yêu cầu thanh toán không hợp lệ." });
        }

        try
        {
            await _guestOrderService.RequestBillAsync(request.TableId, request.Token, ct);
            return Ok(new { success = true, message = "Đã gửi yêu cầu thanh toán thành công! Nhân viên thu ngân và phục vụ sẽ hỗ trợ ngay." });
        }
        catch (DomainValidationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (DomainNotFoundException ex)
        {
            return NotFound(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi thực khách yêu cầu tính tiền bàn {TableId}", request.TableId);
            return StatusCode(500, new { success = false, message = "Lỗi hệ thống khi gửi yêu cầu tính tiền." });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Status([FromQuery] Guid tableId, [FromQuery] string? token, CancellationToken ct)
    {
        try
        {
            var session = await _guestOrderService.GetTableSessionAsync(tableId, token, ct);
            return Json(new { success = true, data = session });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}
