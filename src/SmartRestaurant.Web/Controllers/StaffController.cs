namespace SmartRestaurant.Web.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartRestaurant.Application.Common.Interfaces;
using SmartRestaurant.Application.DTOs;
using SmartRestaurant.Application.Interfaces;

public class StaffController(
    IStaffService staffService,
    IAppDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var branch = await dbContext.Branches.FirstOrDefaultAsync(ct);
        var branchId = branch?.Id ?? Guid.Empty;

        var staff = await staffService.GetStaffMembersAsync(branchId, ct);
        ViewBag.BranchId = branchId;

        return View(staff);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateStaffRequest request, CancellationToken ct)
    {
        try
        {
            await staffService.CreateStaffAsync(request, ct);
            TempData["SuccessMessage"] = $"Đã tạo tài khoản nhân viên '{request.FullName}' thành công.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(UpdateStaffRequest request, CancellationToken ct)
    {
        try
        {
            await staffService.UpdateStaffAsync(request, ct);
            TempData["SuccessMessage"] = "Đã cập nhật thông tin nhân viên thành công.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPin(ResetPinRequest request, CancellationToken ct)
    {
        try
        {
            await staffService.ResetPinAsync(request, ct);
            TempData["SuccessMessage"] = "Đã thiết lập mã PIN mới cho nhân viên.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> VerifyPin([FromBody] VerifyPinRequest request, CancellationToken ct)
    {
        var isValid = await staffService.VerifyPinAsync(request.StaffId, request.PinCode, ct);
        return Json(new { isValid });
    }

    [HttpPost]
    public async Task<IActionResult> AuthorizeManagerPin([FromBody] AuthorizePinRequest request, CancellationToken ct)
    {
        var branch = await dbContext.Branches.FirstOrDefaultAsync(ct);
        var branchId = branch?.Id ?? Guid.Empty;

        var isAuthorized = await staffService.AuthorizeManagerPinAsync(
            branchId,
            request.PinCode,
            request.ActionName,
            request.TargetEntity,
            request.Reason,
            ct);

        return Json(new { isAuthorized });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(Guid id, CancellationToken ct)
    {
        try
        {
            await staffService.ToggleStaffStatusAsync(id, ct);
            TempData["SuccessMessage"] = "Đã thay đổi trạng thái hoạt động của nhân viên.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}

public record VerifyPinRequest(Guid StaffId, string PinCode);
public record AuthorizePinRequest(string PinCode, string ActionName, string TargetEntity, string Reason);
