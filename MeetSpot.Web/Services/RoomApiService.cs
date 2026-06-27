using System.Text.Json;

namespace MeetSpot.Web.Services;

public class RoomResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Floor { get; set; }
    public int Capacity { get; set; }
    public decimal PricePerHour { get; set; }
    public string Type { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public List<string> ImageUrls { get; set; } = new();
    public List<string> Equipment { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public List<string> Errors { get; set; } = new();
    public T? Data { get; set; }
    public int StatusCode { get; set; }
}

public class RoomApiService
{
    private readonly HttpClient _httpClient;

    public RoomApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("MeetSpotAPI");
    }

    public async Task<List<RoomResponseDto>> GetAllRoomsAsync()
    {
        var response = await _httpClient.GetAsync("api/room");
        var json = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var result = JsonSerializer.Deserialize<ApiResponse<List<RoomResponseDto>>>(json, options);

        return result?.Data ?? new List<RoomResponseDto>();
    }
}