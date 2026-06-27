using System.Text.Json;

namespace MeetSpot.Web.Services;

public class PublicStatsDto
{
    public int TotalBookings { get; set; }
    public int TotalRooms { get; set; }
    public int ActiveUsers { get; set; }
    public double OccupancyRate { get; set; }
}

public class AnalyticsApiService
{
    private readonly HttpClient _httpClient;

    public AnalyticsApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("MeetSpotAPI");
    }

    public async Task<PublicStatsDto?> GetPublicStatsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/analytics/public-stats");
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<ApiResponse<PublicStatsDto>>(json, options);

            return result?.Data;
        }
        catch
        {
            return null;
        }
    }
}