namespace SmartRestaurant.Cashier.Web.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartRestaurant.Application.Common.Interfaces;
using SmartRestaurant.Application.Interfaces;
using SmartRestaurant.Domain.Enums;

public class CashierController(
    ICashierService cashierService,
    IVietQrService vietQrService,
    IAppDbContext dbContext) : Controller
{
    private readonly ICashierService _cashierService = cashierService;
    private readonly IVietQrService _vietQrService = vietQrService;
    private readonly IAppDbContext _dbContext = dbContext;

    public async Task<IActionResult> Index()
    {
        var branch = await _dbContext.Branches.FirstOrDefaultAsync();
        var branchId = branch?.Id ?? Guid.Empty;

        var activeBills = await _cashierService.GetActiveBillingTablesAsync(branchId);

        // Check active cashier shift
        var activeShift = await _dbContext.WorkShifts
            .Where(s => s.BranchId == branchId && s.Status == ShiftStatus.Open)
            .OrderByDescending(s => s.OpenedAt)
            .FirstOrDefaultAsync();

        ViewBag.ActiveShift = activeShift;
        ViewBag.BranchId = branchId;

        return View(activeBills);
    }

    public async Task<IActionResult> Bill([FromRoute] Guid id) // id is orderId
    {
        var branch = await _dbContext.Branches.FirstOrDefaultAsync();
        var branchId = branch?.Id ?? Guid.Empty;

        var calc = await _cashierService.GetBillCalculationAsync(id);

        // Get or create standard invoice for quick settlement
        var cashier = await _dbContext.StaffMembers
            .FirstOrDefaultAsync(s => s.BranchId == branchId && s.Role == StaffRole.Cashier);
        var cashierId = cashier?.Id ?? Guid.NewGuid();

        var invoice = await _cashierService.CreateStandardInvoiceAsync(id, cashierId);

        // Generate VietQR dynamic payload
        var vietQr = _vietQrService.GenerateDynamicVietQr(
            invoiceNumber: invoice.InvoiceNumber,
            amount: invoice.TotalAmount
        );

        ViewBag.Invoice = invoice;
        ViewBag.VietQr = vietQr;
        ViewBag.CashierId = cashierId;

        return View(calc);
    }

    public async Task<IActionResult> Split([FromRoute] Guid id) // id is orderId
    {
        var calc = await _cashierService.GetBillCalculationAsync(id);

        var branch = await _dbContext.Branches.FirstOrDefaultAsync();
        var branchId = branch?.Id ?? Guid.Empty;
        var cashier = await _dbContext.StaffMembers
            .FirstOrDefaultAsync(s => s.BranchId == branchId && s.Role == StaffRole.Cashier);

        ViewBag.CashierId = cashier?.Id ?? Guid.NewGuid();

        return View(calc);
    }

    public async Task<IActionResult> Receipt([FromRoute] Guid id) // id is invoiceId
    {
        var invoice = await _cashierService.GetInvoiceByIdAsync(id);
        var branch = await _dbContext.Branches.FirstOrDefaultAsync();

        ViewBag.Branch = branch;
        return View(invoice);
    }

    public async Task<IActionResult> Shift()
    {
        var branch = await _dbContext.Branches.FirstOrDefaultAsync();
        var branchId = branch?.Id ?? Guid.Empty;

        var activeShift = await _dbContext.WorkShifts
            .Where(s => s.BranchId == branchId && s.Status == ShiftStatus.Open)
            .OrderByDescending(s => s.OpenedAt)
            .FirstOrDefaultAsync();

        var recentVarianceLogs = await _dbContext.ShiftClosingVarianceLogs
            .Where(l => l.BranchId == branchId)
            .OrderByDescending(l => l.LoggedAt)
            .Take(10)
            .ToListAsync();

        ViewBag.RecentLogs = recentVarianceLogs;
        ViewBag.BranchId = branchId;

        return View(activeShift);
    }

    public async Task<IActionResult> Suspense()
    {
        var branch = await _dbContext.Branches.FirstOrDefaultAsync();
        var branchId = branch?.Id ?? Guid.Empty;

        var items = await _cashierService.GetActiveSuspenseItemsAsync(branchId);
        return View(items);
    }
}
