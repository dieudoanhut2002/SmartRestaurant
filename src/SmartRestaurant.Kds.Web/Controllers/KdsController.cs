namespace SmartRestaurant.Kds.Web.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartRestaurant.Application.Common.Interfaces;
using SmartRestaurant.Application.Interfaces;

public class KdsController(
    IKitchenService kitchenService,
    IAppDbContext dbContext) : Controller
{
    private readonly IKitchenService _kitchenService = kitchenService;
    private readonly IAppDbContext _dbContext = dbContext;

    public async Task<IActionResult> Index([FromQuery] Guid? stationId)
    {
        var branch = await _dbContext.Branches.FirstOrDefaultAsync();
        var branchId = branch?.Id ?? Guid.Empty;

        var stations = await _dbContext.KitchenStations
            .Where(s => s.BranchId == branchId && s.IsActive)
            .OrderBy(s => s.Code)
            .ToListAsync();

        var selectedStation = stationId.HasValue
            ? stations.FirstOrDefault(s => s.Id == stationId.Value)
            : stations.FirstOrDefault();

        var selectedStationId = selectedStation?.Id;

        var tickets = await _kitchenService.GetActiveTicketsAsync(branchId, selectedStationId);
        var summaries = await _kitchenService.GetStationsSummaryAsync(branchId);

        ViewBag.BranchId = branchId;
        ViewBag.Stations = stations;
        ViewBag.SelectedStation = selectedStation;
        ViewBag.SelectedStationId = selectedStationId;
        ViewBag.Summaries = summaries;

        return View(tickets);
    }

    public async Task<IActionResult> Aggregated([FromQuery] Guid? stationId)
    {
        var branch = await _dbContext.Branches.FirstOrDefaultAsync();
        var branchId = branch?.Id ?? Guid.Empty;

        var stations = await _dbContext.KitchenStations
            .Where(s => s.BranchId == branchId && s.IsActive)
            .OrderBy(s => s.Code)
            .ToListAsync();

        var selectedStation = stationId.HasValue
            ? stations.FirstOrDefault(s => s.Id == stationId.Value)
            : stations.FirstOrDefault();

        var selectedStationId = selectedStation?.Id;

        var queue = await _kitchenService.GetAggregatedQueueAsync(branchId, selectedStationId);

        ViewBag.BranchId = branchId;
        ViewBag.Stations = stations;
        ViewBag.SelectedStation = selectedStation;
        ViewBag.SelectedStationId = selectedStationId;

        return View(queue);
    }

    public async Task<IActionResult> Expediter()
    {
        var branch = await _dbContext.Branches.FirstOrDefaultAsync();
        var branchId = branch?.Id ?? Guid.Empty;

        var stations = await _dbContext.KitchenStations
            .Where(s => s.BranchId == branchId && s.IsActive)
            .OrderBy(s => s.Code)
            .ToListAsync();

        // Expediter views all active tickets across all stations
        var tickets = await _kitchenService.GetActiveTicketsAsync(branchId, null);

        ViewBag.BranchId = branchId;
        ViewBag.Stations = stations;

        return View(tickets);
    }
}
