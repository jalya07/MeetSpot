using Microsoft.EntityFrameworkCore;
using MeetSpot.Core.Entities;
using MeetSpot.Core.Interfaces;
using MeetSpot.Infrastructure.Data;

namespace MeetSpot.API.BackgroundServices;

public class BookingAutoReleaseService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<BookingAutoReleaseService> _logger;

    public BookingAutoReleaseService(
        IServiceScopeFactory scopeFactory,
        ILogger<BookingAutoReleaseService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await CheckAndReleaseBookings();
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }

    private async Task CheckAndReleaseBookings()
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MeetSpotAppDbContext>();
        var notificationService = scope.ServiceProvider
            .GetRequiredService<IBookingNotificationService>();

        var cutoffTime = DateTime.UtcNow.AddMinutes(-15);

        var noShowBookings = await context.Bookings
            .Include(b => b.Room)
            .Where(b => b.Status == BookingStatus.Active
                && !b.IsCheckedIn
                && b.StartTime <= cutoffTime)
            .ToListAsync();

        foreach (var booking in noShowBookings)
        {
            booking.Status = BookingStatus.NoShow;
            booking.UpdatedAt = DateTime.UtcNow;

            _logger.LogInformation(
                "Auto-released booking {BookingId} for room {RoomName}",
                booking.Id, booking.Room.Name);

            await notificationService.NotifyBookingCancelled(
                booking.RoomId, booking.Room.Name);
        }

        if (noShowBookings.Any())
            await context.SaveChangesAsync();
    }
}