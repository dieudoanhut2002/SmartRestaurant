namespace SmartRestaurant.Web.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartRestaurant.Application.Common.Interfaces;
using SmartRestaurant.Application.DTOs;
using SmartRestaurant.Application.Interfaces;

public class InventoryController(
    IInventoryService inventoryService,
    IAppDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var branch = await dbContext.Branches.FirstOrDefaultAsync(ct);
        var branchId = branch?.Id ?? Guid.Empty;

        var summary = await inventoryService.GetSummaryAsync(branchId, ct);
        var items = await inventoryService.GetInventoryItemsAsync(branchId, ct);
        var transactions = await inventoryService.GetTransactionsAsync(branchId, cancellationToken: ct);

        ViewBag.BranchId = branchId;
        ViewBag.Summary = summary;
        ViewBag.Transactions = transactions;

        return View(items);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Receive(ReceiveStockRequest request, CancellationToken ct)
    {
        try
        {
            await inventoryService.ReceiveStockAsync(request, ct);
            TempData["SuccessMessage"] = $"Đã nhập kho thêm {request.Quantity} sản phẩm thành công.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Adjust(AdjustStockRequest request, CancellationToken ct)
    {
        try
        {
            await inventoryService.AdjustStockAsync(request, ct);
            TempData["SuccessMessage"] = "Đã cập nhật số lượng kiểm kê thực tế thành công.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateInventoryItemRequest request, CancellationToken ct)
    {
        try
        {
            await inventoryService.CreateItemAsync(request, ct);
            TempData["SuccessMessage"] = $"Đã tạo mới mặt hàng kho '{request.ItemName}' thành công.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> GetTransactions(Guid itemId, CancellationToken ct)
    {
        var branch = await dbContext.Branches.FirstOrDefaultAsync(ct);
        var branchId = branch?.Id ?? Guid.Empty;

        var txs = await inventoryService.GetTransactionsAsync(branchId, itemId, ct);
        return Json(txs);
    }
}
