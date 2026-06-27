namespace MeetSpot.Core.DTOs.Analytics;

public class PublicStatsDto
{
    public int TotalBookings { get; set; }
    public int TotalRooms { get; set; }
    public int ActiveUsers { get; set; }
    public double OccupancyRate { get; set; }
}