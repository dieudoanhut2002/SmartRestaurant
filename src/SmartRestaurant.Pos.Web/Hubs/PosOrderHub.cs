namespace SmartRestaurant.Pos.Web.Hubs;

using Microsoft.AspNetCore.SignalR;

public class PosOrderHub : Hub
{
    public async Task JoinTableGroup(string tableId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"table_{tableId}");
    }

    public async Task LeaveTableGroup(string tableId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"table_{tableId}");
    }
}
