using Microsoft.AspNetCore.SignalR;

namespace MeetSpot.API.Hubs;

public class BookingHub : Hub
{
     public override async Task OnConnectedAsync()
    {
        Console.WriteLine($"=== Client connected: {Context.ConnectionId} ===");
        await base.OnConnectedAsync();
    }
}