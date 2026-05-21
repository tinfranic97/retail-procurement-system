using Microsoft.AspNetCore.SignalR;

namespace RetailProcurement.API.Hubs;

public class ProcurementHub : Hub
{
    public async Task JoinGroup(string group)
        => await Groups.AddToGroupAsync(Context.ConnectionId, group);

    public async Task LeaveGroup(string group)
        => await Groups.RemoveFromGroupAsync(Context.ConnectionId, group);

    public override async Task OnConnectedAsync()
    {
        await Clients.Caller.SendAsync("Connected", Context.ConnectionId);
        await base.OnConnectedAsync();
    }
}
