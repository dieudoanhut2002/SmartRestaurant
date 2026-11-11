namespace SmartRestaurant.Web.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartRestaurant.Application.Common.Interfaces;
using SmartRestaurant.Application.DTOs;
using SmartRestaurant.Application.Interfaces;

public class MenuController(
    IMenuService menuService,
    IAppDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index(Guid? categoryId, string? search, CancellationToken ct)
    {
        var branch = await dbContext.Branches.FirstOrDefaultAsync(ct);
        var branchId = branch?.Id ?? Guid.Empty;

        var categories = await menuService.GetCategoriesAsync(branchId, ct);
        var items = await menuService.GetMenuItemsAsync(branchId, categoryId, search, ct);
        var modifierGroups = await menuService.GetModifierGroupsAsync(branchId, ct);

        ViewBag.Categories = categories;
        ViewBag.ModifierGroups = modifierGroups;
        ViewBag.SelectedCategoryId = categoryId;
        ViewBag.SearchTerm = search;
        ViewBag.BranchId = branchId;

        return View(items);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateModifierGroup(CreateModifierGroupRequest request, CancellationToken ct)
    {
        try
        {
            await menuService.CreateModifierGroupAsync(request, ct);
            TempData["SuccessMessage"] = $"Đã tạo nhóm tùy chọn '{request.Name}' thành công.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddModifierOption(CreateModifierOptionRequest request, CancellationToken ct)
    {
        try
        {
            await menuService.AddModifierOptionAsync(request, ct);
            TempData["SuccessMessage"] = $"Đã thêm tùy chọn '{request.Name}' thành công.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteModifierGroup(Guid id, CancellationToken ct)
    {
        try
        {
            await menuService.DeleteModifierGroupAsync(id, ct);
            TempData["SuccessMessage"] = "Đã xóa nhóm tùy chọn thành công.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddVariant(CreateVariantRequest request, Guid? categoryId, CancellationToken ct)
    {
        try
        {
            await menuService.AddVariantAsync(request, ct);
            TempData["SuccessMessage"] = $"Đã thêm biến thể '{request.Name}' thành công.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index), new { categoryId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteVariant(Guid variantId, Guid? categoryId, CancellationToken ct)
    {
        try
        {
            await menuService.DeleteVariantAsync(variantId, ct);
            TempData["SuccessMessage"] = "Đã xóa biến thể món thành công.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index), new { categoryId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateItem(CreateMenuItemRequest request, CancellationToken ct)
    {
        try
        {
            await menuService.CreateMenuItemAsync(request, ct);
            TempData["SuccessMessage"] = $"Đã thêm món '{request.Name}' vào thực đơn.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index), new { categoryId = request.CategoryId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateItem(UpdateMenuItemRequest request, CancellationToken ct)
    {
        try
        {
            await menuService.UpdateMenuItemAsync(request, ct);
            TempData["SuccessMessage"] = "Đã cập nhật thông tin món ăn thành công.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index), new { categoryId = request.CategoryId });
    }

    [HttpPost]
    public async Task<IActionResult> ToggleAvailability(Guid id, CancellationToken ct)
    {
        var success = await menuService.ToggleAvailabilityAsync(id, ct);
        return Json(new { success });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateStock([FromBody] UpdateStockRequest request, CancellationToken ct)
    {
        var success = await menuService.UpdateStockAsync(request.MenuItemId, request.Quantity, ct);
        return Json(new { success });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCategory(CreateCategoryRequest request, CancellationToken ct)
    {
        try
        {
            var category = await menuService.CreateCategoryAsync(request, ct);
            TempData["SuccessMessage"] = $"Đã thêm nhóm thực đơn '{request.Name}'.";
            return RedirectToAction(nameof(Index), new { categoryId = category.Id });
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteItem(Guid id, Guid? categoryId, CancellationToken ct)
    {
        try
        {
            await menuService.DeleteMenuItemAsync(id, ct);
            TempData["SuccessMessage"] = "Đã xóa món ăn khỏi thực đơn.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index), new { categoryId });
    }
}

public record UpdateStockRequest(Guid MenuItemId, int Quantity);
