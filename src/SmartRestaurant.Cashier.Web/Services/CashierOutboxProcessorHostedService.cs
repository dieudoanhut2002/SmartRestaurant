namespace SmartRestaurant.Cashier.Web.Services;

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SmartRestaurant.Application.Common.Interfaces;
using SmartRestaurant.Cashier.Web.Hubs;

public class CashierOutboxProcessorHostedService(
    IServiceScopeFactory scopeFactory,
    IHubContext<CashierHub> cashierHub,
    ILogger<CashierOutboxProcessorHostedService> logger) : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly IHubContext<CashierHub> _cashierHub = cashierHub;
    private readonly ILogger<CashierOutboxProcessorHostedService> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Cashier Outbox Processor Service đã khởi động.");

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
                            case "TableStatusChanged":
                                await _cashierHub.Clients.All.SendAsync("TableStatusChanged", msg.PayloadJson, stoppingToken);
                                break;

                            case "InvoiceSettled":
                                await _cashierHub.Clients.All.SendAsync("InvoiceSettled", msg.PayloadJson, stoppingToken);
                                break;
                        }

                        msg.MarkProcessed();
                    }

                    await dbContext.SaveChangesAsync(stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xử lý Outbox messages trong Cashier.");
            }

            await Task.Delay(1000, stoppingToken);
        }
    }
}
