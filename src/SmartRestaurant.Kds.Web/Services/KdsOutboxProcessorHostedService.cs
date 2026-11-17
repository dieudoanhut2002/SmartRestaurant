namespace SmartRestaurant.Kds.Web.Services;

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SmartRestaurant.Application.Common.Interfaces;
using SmartRestaurant.Kds.Web.Hubs;

public class KdsOutboxProcessorHostedService(
    IServiceScopeFactory scopeFactory,
    IHubContext<KitchenHub> kitchenHub,
    ILogger<KdsOutboxProcessorHostedService> logger) : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly IHubContext<KitchenHub> _kitchenHub = kitchenHub;
    private readonly ILogger<KdsOutboxProcessorHostedService> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("KDS Outbox Processor Service đã khởi động.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();

                var pendingMessages = await dbContext.OutboxMessages
                    .Where(m => m.ProcessedAt == null)
                    .OrderBy(m => m.CreatedAt)
                    .Take(25)
                    .ToListAsync(stoppingToken);

                if (pendingMessages.Count > 0)
                {
                    foreach (var msg in pendingMessages)
                    {
                        switch (msg.EventType)
                        {
                            case "OrderFired":
                                await _kitchenHub.Clients.All.SendAsync("OrderFired", msg.PayloadJson, stoppingToken);
                                break;

                            case "KitchenTicketUpdated":
                                await _kitchenHub.Clients.All.SendAsync("KitchenTicketUpdated", msg.PayloadJson, stoppingToken);
                                break;

                            case "OrderItemStatusChanged":
                                await _kitchenHub.Clients.All.SendAsync("OrderItemStatusChanged", msg.PayloadJson, stoppingToken);
                                break;

                            case "Item86Toggled":
                                await _kitchenHub.Clients.All.SendAsync("Item86Toggled", msg.PayloadJson, stoppingToken);
                                break;
                        }

                        msg.MarkProcessed();
                    }

                    await dbContext.SaveChangesAsync(stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xử lý Outbox messages trong KDS.");
            }

            await Task.Delay(1000, stoppingToken);
        }
    }
}
