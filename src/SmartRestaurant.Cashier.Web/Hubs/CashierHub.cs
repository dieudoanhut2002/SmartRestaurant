namespace SmartRestaurant.Cashier.Web.Hubs;

using Microsoft.AspNetCore.SignalR;

public class CashierHub : Hub
{
    public async Task JoinCashierGroup()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "cashier_group");
    }

    public async Task LeaveCashierGroup()
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "cashier_group");
    }
}
