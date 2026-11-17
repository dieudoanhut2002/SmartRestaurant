namespace SmartRestaurant.Cashier.Web.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartRestaurant.Application.Common.Interfaces;
using SmartRestaurant.Application.DTOs;
using SmartRestaurant.Application.Interfaces;
using SmartRestaurant.Domain.Entities;
using SmartRestaurant.Domain.Enums;

[ApiController]
[Route("api/payment")]
public class PaymentApiController(
    ICashierService cashierService,
    IVietQrService vietQrService,
    IAppDbContext dbContext) : ControllerBase
{
    private readonly ICashierService _cashierService = cashierService;
    private readonly IVietQrService _vietQrService = vietQrService;
    private readonly IAppDbContext _dbContext = dbContext;

    [HttpPost("settle")]
    public async Task<IActionResult> Settle([FromBody] SettleInvoiceRequest request)
    {
        try
        {
            var result = await _cashierService.SettleInvoiceAsync(request);
            return Ok(new { success = true, invoice = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost("split-bill")]
    public async Task<IActionResult> SplitBill([FromBody] SplitBillRequest request)
    {
        try
        {
            var invoices = await _cashierService.SplitBillAsync(request);
            return Ok(new { success = true, invoices });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("vietqr/{invoiceNumber}")]
    public IActionResult GetVietQr([FromRoute] string invoiceNumber, [FromQuery] decimal amount)
    {
        var payload = _vietQrService.GenerateDynamicVietQr(invoiceNumber, amount);
        return Ok(payload);
    }

    [HttpPost("vietqr-webhook")]
    public async Task<IActionResult> ProcessVietQrWebhook([FromBody] ReconcileWebhookRequest request)
    {
        try
        {
            var success = await _cashierService.ProcessVietQrWebhookAsync(request);
            return Ok(new { success });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost("blind-shift-close")]
    public async Task<IActionResult> BlindShiftClose([FromBody] BlindShiftCloseRequest request)
    {
        try
        {
            var result = await _cashierService.CloseShiftBlindAsync(request);
            return Ok(new { success = true, data = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost("open-shift")]
    public async Task<IActionResult> OpenShift([FromBody] OpenShiftRequest request)
    {
        try
        {
            var branch = await _dbContext.Branches.FirstOrDefaultAsync();
            var branchId = branch?.Id ?? Guid.Empty;

            var existingOpen = await _dbContext.WorkShifts
                .FirstOrDefaultAsync(s => s.BranchId == branchId && s.Status == ShiftStatus.Open);

            if (existingOpen != null)
                return BadRequest(new { success = false, message = "Đang có một ca làm việc đang mở. Vui lòng đóng ca trước khi mở ca mới." });

            var shift = new WorkShift(branchId, request.StaffId, request.OpeningCashFloat, request.Notes ?? "");
            _dbContext.WorkShifts.Add(shift);
            await _dbContext.SaveChangesAsync();

            return Ok(new { success = true, shiftId = shift.Id });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}

public record OpenShiftRequest(
    Guid StaffId,
    decimal OpeningCashFloat,
    string? Notes);
