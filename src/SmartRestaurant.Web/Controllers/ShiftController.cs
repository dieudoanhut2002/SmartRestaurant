namespace SmartRestaurant.Web.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartRestaurant.Application.Common.Interfaces;
using SmartRestaurant.Application.DTOs;
using SmartRestaurant.Application.Interfaces;
using SmartRestaurant.Domain.Enums;

public class ShiftController(
    IShiftService shiftService,
    IStaffService staffService,
    IAppDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var branch = await dbContext.Branches.FirstOrDefaultAsync(ct);
        var branchId = branch?.Id ?? Guid.Empty;

        var activeShifts = await shiftService.GetActiveShiftsAsync(branchId, ct);
        var history = await shiftService.GetShiftHistoryAsync(branchId, 1, 15, ct);
        var staff = await staffService.GetStaffMembersAsync(branchId, ct);

        ViewBag.ActiveShifts = activeShifts;
        ViewBag.StaffList = staff.Where(s => s.Role == StaffRole.Cashier || s.Role == StaffRole.RestaurantManager).ToList();
        ViewBag.BranchId = branchId;

        return View(history);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> OpenShift(OpenShiftRequest request, CancellationToken ct)
    {
        try
        {
            await shiftService.OpenShiftAsync(request, ct);
            TempData["SuccessMessage"] = "Đã mở ca làm việc mới thành công.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CloseShift(CloseShiftBlindRequest request, CancellationToken ct)
    {
        try
        {
            var log = await shiftService.CloseShiftBlindAsync(request, ct);
            var varianceStr = log.VarianceAmount == 0
                ? "Khớp chính xác (0 ₫)"
                : log.VarianceAmount > 0
                    ? $"Dư +{log.VarianceAmount:N0} ₫"
                    : $"Thâm hụt {log.VarianceAmount:N0} ₫";

            TempData["SuccessMessage"] = $"Đã chốt ca thành công! Kết quả kiểm tiền mù: {varianceStr}.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> VarianceLogs(CancellationToken ct)
    {
        var branch = await dbContext.Branches.FirstOrDefaultAsync(ct);
        var branchId = branch?.Id ?? Guid.Empty;

        var logs = await shiftService.GetVarianceLogsAsync(branchId, ct);
        return View(logs);
    }
}
