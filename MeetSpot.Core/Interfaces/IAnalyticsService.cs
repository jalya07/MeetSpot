using MeetSpot.Core.DTOs.Analytics;

namespace MeetSpot.Core.Interfaces;

public interface IAnalyticsService
{
    Task<AnalyticsDto> GetDashboardAnalyticsAsync();
}