using Microsoft.EntityFrameworkCore;
using MeetSpot.Core.DTOs.Analytics;
using MeetSpot.Core.Entities;
using MeetSpot.Core.Interfaces;
using MeetSpot.Infrastructure.Data;

namespace MeetSpot.Infrastructure.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly MeetSpotAppDbContext _context;

    public AnalyticsService(MeetSpotAppDbContext context)
    {
        _context = context;
    }

    public async Task<AnalyticsDto> GetDashboardAnalyticsAsync()
    {
        var totalRevenue = await _context.Transactions
            .Where(t => t.Type == TransactionType.BookingPayment || t.Type == TransactionType.SubscriptionPayment)
            .SumAsync(t => -t.Amount);

        var totalBookings = await _context.Bookings.CountAsync();

        var activeUsers = await _context.Users
            .Where(u => u.Role != UserRole.Admin && u.IsActive)
            .CountAsync();

        var totalRooms = await _context.Rooms.Where(r => r.IsActive).CountAsync();

        var topRooms = await _context.Bookings
            .GroupBy(b => b.Room.Name)
            .Select(g => new RoomPopularityDto
            {
                RoomName = g.Key,
                BookingCount = g.Count()
            })
            .OrderByDescending(r => r.BookingCount)
            .Take(5)
            .ToListAsync();

        var allBookings = await _context.Bookings
            .Select(b => b.StartTime.Hour)
            .ToListAsync();

        var peakHours = allBookings
            .GroupBy(h => h)
            .Select(g => new PeakHourDto
            {
                Hour = g.Key,
                BookingCount = g.Count()
            })
            .OrderByDescending(h => h.BookingCount)
            .Take(5)
            .ToList();

        var totalRoomHours = totalRooms * 24 * 30;
        var bookingTimes = await _context.Bookings
            .Where(b => b.Status == BookingStatus.Active || b.Status == BookingStatus.Completed)
            .Select(b => new { b.StartTime, b.EndTime })
            .ToListAsync();

        var bookedHours = bookingTimes.Sum(b => (b.EndTime - b.StartTime).TotalHours);

        var occupancyRate = totalRoomHours > 0
            ? Math.Round((bookedHours / totalRoomHours) * 100, 2)
            : 0;

        return new AnalyticsDto
        {
            TotalRevenue = totalRevenue,
            TotalBookings = totalBookings,
            ActiveUsers = activeUsers,
            TotalRooms = totalRooms,
            TopRooms = topRooms,
            PeakHours = peakHours,
            OccupancyRate = occupancyRate
        };
    }

    public async Task<PublicStatsDto> GetPublicStatsAsync()
    {
        var totalBookings = await _context.Bookings.CountAsync();
        var totalRooms = await _context.Rooms.Where(r => r.IsActive).CountAsync();
        var activeUsers = await _context.Users
            .Where(u => u.Role != UserRole.Admin && u.IsActive)
            .CountAsync();

        var totalRoomHours = totalRooms * 24 * 30;
        var bookingTimes = await _context.Bookings
            .Where(b => b.Status == BookingStatus.Active || b.Status == BookingStatus.Completed)
            .Select(b => new { b.StartTime, b.EndTime })
            .ToListAsync();
        var bookedHours = bookingTimes.Sum(b => (b.EndTime - b.StartTime).TotalHours);
        var occupancyRate = totalRoomHours > 0 ? Math.Round((bookedHours / totalRoomHours) * 100, 2) : 0;

        return new PublicStatsDto
        {
            TotalBookings = totalBookings,
            TotalRooms = totalRooms,
            ActiveUsers = activeUsers,
            OccupancyRate = occupancyRate
        };
    }
}