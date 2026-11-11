namespace SmartRestaurant.Web.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartRestaurant.Application.Common.Interfaces;
using SmartRestaurant.Application.DTOs;
using SmartRestaurant.Application.Interfaces;

public class SettingController(
    ISettingService settingService,
    IAppDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var branch = await dbContext.Branches.FirstOrDefaultAsync(ct);
        var branchId = branch?.Id ?? Guid.Empty;

        var settings = await settingService.GetSettingsAsync(branchId, ct);
        return View(settings);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateCutoff(UpdateBusinessDayCutoffRequest request, CancellationToken ct)
    {
        try
        {
            await settingService.UpdateCutoffAsync(request, ct);
            TempData["SuccessMessage"] = $"Đã cập nhật giờ cắt ngày kinh doanh thành {request.CutoffTime:hh\\:mm} thành công.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateVipSurcharges(UpdateVipSurchargeRequest request, CancellationToken ct)
    {
        try
        {
            await settingService.UpdateVipSurchargesAsync(request, ct);
            TempData["SuccessMessage"] = "Đã cập nhật cấu hình phụ thu phòng VIP thành công.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfigureModules(UpdateModuleConfigRequest request, CancellationToken ct)
    {
        try
        {
            await settingService.ConfigureModulesAsync(request, ct);
            TempData["SuccessMessage"] = "Đã lưu cấu hình các module chức năng thành công.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ApplyPreset(ApplyPresetRequest request, CancellationToken ct)
    {
        try
        {
            await settingService.ApplyPresetAsync(request, ct);
            TempData["SuccessMessage"] = $"Đã áp dụng mẫu cấu hình vận hành '{request.PresetName}' thành công.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}
