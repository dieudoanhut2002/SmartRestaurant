namespace SmartRestaurant.Pos.Web.Hubs;

using Microsoft.AspNetCore.SignalR;

public class TableHub : Hub
{
    public async Task JoinBranchGroup(string branchId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"branch_{branchId}");
    }

    public async Task LeaveBranchGroup(string branchId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"branch_{branchId}");
    }
}
