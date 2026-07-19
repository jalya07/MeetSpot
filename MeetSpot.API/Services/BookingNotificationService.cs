using Microsoft.AspNetCore.SignalR;
using MeetSpot.API.Hubs;
using MeetSpot.Core.Interfaces;

namespace MeetSpot.API.Services;

public class BookingNotificationService : IBookingNotificationService
{
    private readonly IHubContext<BookingHub> _hubContext;

    public BookingNotificationService(IHubContext<BookingHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyBookingCreated(int roomId, string roomName, DateTime startTime, DateTime endTime)
    {
        Console.WriteLine($"=== SignalR: Sending BookingCreated for {roomName} ===");
        await _hubContext.Clients.All.SendAsync("BookingCreated", new
        {
            roomId,
            roomName,
            startTime,
            endTime,
            message = $"{roomName} has been booked from {startTime:HH:mm} to {endTime:HH:mm}"
        });
    }
    
    public async Task NotifyBookingCancelled(int roomId, string roomName)
    {
        
        await _hubContext.Clients.All.SendAsync("BookingCancelled", new
        {
            roomId,
            roomName,
            message = $"{roomName} is now available"
        });
    }
}