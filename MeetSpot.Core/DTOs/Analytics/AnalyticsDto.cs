namespace MeetSpot.Core.DTOs.Analytics;

public class AnalyticsDto
{
    public decimal TotalRevenue { get; set; }
    public int TotalBookings { get; set; }
    public int ActiveUsers { get; set; }
    public int TotalRooms { get; set; }
    public List<RoomPopularityDto> TopRooms { get; set; } = new();
    public List<PeakHourDto> PeakHours { get; set; } = new();
    public double OccupancyRate { get; set; }
    public int[] BookingsByDayOfWeek { get; set; } = new int[7];
}

public class RoomPopularityDto
{
    public string RoomName { get; set; } = string.Empty;
    public int BookingCount { get; set; }
}

public class PeakHourDto
{
    public int Hour { get; set; }
    public int BookingCount { get; set; }
}