namespace SmartRestaurant.Kds.Web.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartRestaurant.Application.Common.Interfaces;
using SmartRestaurant.Application.DTOs;
using SmartRestaurant.Application.Interfaces;

[ApiController]
[Route("api/kds")]
public class KdsApiController(
    IKitchenService kitchenService,
    IAppDbContext dbContext) : ControllerBase
{
    private readonly IKitchenService _kitchenService = kitchenService;
    private readonly IAppDbContext _dbContext = dbContext;

    [HttpGet("stations")]
    public async Task<IActionResult> GetStations()
    {
        var branch = await _dbContext.Branches.FirstOrDefaultAsync();
        var branchId = branch?.Id ?? Guid.Empty;
        var summaries = await _kitchenService.GetStationsSummaryAsync(branchId);
        return Ok(summaries);
    }

    [HttpGet("tickets")]
    public async Task<IActionResult> GetTickets([FromQuery] Guid? stationId)
    {
        var branch = await _dbContext.Branches.FirstOrDefaultAsync();
        var branchId = branch?.Id ?? Guid.Empty;
        var tickets = await _kitchenService.GetActiveTicketsAsync(branchId, stationId);
        return Ok(tickets);
    }

    [HttpGet("aggregated")]
    public async Task<IActionResult> GetAggregated([FromQuery] Guid? stationId)
    {
        var branch = await _dbContext.Branches.FirstOrDefaultAsync();
        var branchId = branch?.Id ?? Guid.Empty;
        var queue = await _kitchenService.GetAggregatedQueueAsync(branchId, stationId);
        return Ok(queue);
    }

    [HttpPost("ticket-status")]
    public async Task<IActionResult> UpdateTicketStatus([FromBody] UpdateTicketStatusRequest request)
    {
        try
        {
            var result = await _kitchenService.UpdateTicketStatusAsync(request);
            return Ok(new { success = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost("item-status")]
    public async Task<IActionResult> UpdateItemStatus([FromBody] UpdateItemStatusRequest request)
    {
        try
        {
            var result = await _kitchenService.UpdateOrderItemStatusAsync(request);
            return Ok(new { success = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost("remake")]
    public async Task<IActionResult> RemakeDish([FromBody] RemakeDishRequest request)
    {
        try
        {
            var result = await _kitchenService.RemakeDishAsync(request);
            return Ok(new { success = true, ticket = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost("toggle-86")]
    public async Task<IActionResult> Toggle86([FromBody] Toggle86Request request)
    {
        try
        {
            var result = await _kitchenService.ToggleItem86Async(request);
            return Ok(new { success = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}
