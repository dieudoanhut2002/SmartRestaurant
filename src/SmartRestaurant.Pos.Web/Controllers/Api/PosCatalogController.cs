namespace SmartRestaurant.Pos.Web.Controllers.Api;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartRestaurant.Application.Common.Interfaces;
using SmartRestaurant.Application.Interfaces;

[ApiController]
[Route("api/pos")]
public class PosCatalogController(
    IPosOrderService posOrderService,
    IAppDbContext dbContext) : ControllerBase
{
    private readonly IPosOrderService _posOrderService = posOrderService;
    private readonly IAppDbContext _dbContext = dbContext;

    [HttpGet("menu-catalog")]
    public async Task<IActionResult> GetCatalog([FromQuery] Guid? branchId)
    {
        var bid = branchId;
        if (!bid.HasValue || bid.Value == Guid.Empty)
        {
            var branch = await _dbContext.Branches.FirstOrDefaultAsync();
            bid = branch?.Id ?? Guid.Empty;
        }

        var catalog = await _posOrderService.GetMenuCatalogAsync(bid.Value);
        return Ok(catalog);
    }

    [HttpGet("sync/{tableId:guid}")]
    public async Task<IActionResult> SyncTableState(Guid tableId)
    {
        try
        {
            var syncData = await _posOrderService.GetTableStateSyncAsync(tableId);
            return Ok(new { success = true, data = syncData });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}
