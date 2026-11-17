namespace SmartRestaurant.Pos.Web.Hubs;

using Microsoft.AspNetCore.SignalR;

public class KitchenHub : Hub
{
    public async Task JoinStationGroup(string stationId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"station_{stationId}");
    }

    public async Task LeaveStationGroup(string stationId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"station_{stationId}");
    }
}
