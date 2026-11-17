namespace SmartRestaurant.Pos.Web.Services;

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SmartRestaurant.Application.Common.Interfaces;
using SmartRestaurant.Pos.Web.Hubs;

public class OutboxProcessorHostedService(
    IServiceScopeFactory scopeFactory,
    IHubContext<TableHub> tableHub,
    IHubContext<KitchenHub> kitchenHub,
    IHubContext<PosOrderHub> orderHub,
    ILogger<OutboxProcessorHostedService> logger) : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly IHubContext<TableHub> _tableHub = tableHub;
    private readonly IHubContext<KitchenHub> _kitchenHub = kitchenHub;
    private readonly IHubContext<PosOrderHub> _orderHub = orderHub;
    private readonly ILogger<OutboxProcessorHostedService> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Outbox Processor Service đã khởi động.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();

                var pendingMessages = await dbContext.OutboxMessages
                    .Where(m => m.ProcessedAt == null)
                    .OrderBy(m => m.CreatedAt)
                    .Take(20)
                    .ToListAsync(stoppingToken);

                if (pendingMessages.Count > 0)
                {
                    foreach (var msg in pendingMessages)
                    {
                        switch (msg.EventType)
                        {
                            case "TableStatusChanged":
                                await _tableHub.Clients.All.SendAsync("TableStatusChanged", msg.PayloadJson, stoppingToken);
                                break;

                            case "OrderFired":
                                await _kitchenHub.Clients.All.SendAsync("OrderFired", msg.PayloadJson, stoppingToken);
                                await _orderHub.Clients.All.SendAsync("OrderFired", msg.PayloadJson, stoppingToken);
                                break;

                            case "OrderItemStatusChanged":
                                await _orderHub.Clients.All.SendAsync("OrderItemStatusChanged", msg.PayloadJson, stoppingToken);
                                break;
                        }

                        msg.MarkProcessed();
                    }

                    await dbContext.SaveChangesAsync(stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xử lý Outbox messages.");
            }

            await Task.Delay(1500, stoppingToken);
        }
    }
}
