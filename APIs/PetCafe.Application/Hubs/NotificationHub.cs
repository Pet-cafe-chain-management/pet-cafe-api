using Microsoft.AspNetCore.SignalR;

namespace PetCafe.Application.Hubs;

public class NotificationHub : Hub
{
    // Method để client join vào group theo AccountId
    public async Task JoinUserGroup(string accountId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{accountId}");
    }

    // Method để client leave khỏi group
    public async Task LeaveUserGroup(string accountId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{accountId}");
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }
}

