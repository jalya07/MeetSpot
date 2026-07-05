using System.Text.Json;
using System.Net.Http.Headers;

namespace MeetSpot.Web.Services;

public class BookingResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string RoomName { get; set; } = string.Empty;
    public string? RoomImageUrl { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsCheckedIn { get; set; }
    public string? QrCode { get; set; }
    public int RoomId { get; set; }
}

public class BookingApiService
{
    private readonly HttpClient _httpClient;

    public BookingApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("MeetSpotAPI");
    }

    public async Task<List<BookingResponseDto>> GetMyBookingsAsync(string token)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync("api/booking/my");
            if (!response.IsSuccessStatusCode) return new();

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<ApiResponse<List<BookingResponseDto>>>(json, options);

            return result?.Data ?? new();
        }
        catch
        {
            return new();
        }
    }

    public async Task<(bool Success, List<string> Errors)> CreateBookingAsync(string token, int roomId, string title, DateTime startTime, DateTime endTime)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var bookingData = new
            {
                RoomId = roomId,
                Title = title,
                StartTime = startTime,
                EndTime = endTime,
                ParticipantEmails = new List<string>()
            };

            var json = JsonSerializer.Serialize(bookingData);
            var jsonContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/booking", jsonContent);
            var jsonResponse = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<ApiResponse<object>>(jsonResponse, options);

            return response.IsSuccessStatusCode
                ? (true, new List<string>())
                : (false, result?.Errors ?? new List<string> { "Booking failed" });
        }
        catch
        {
            return (false, new List<string> { "Something went wrong" });
        }
    }
}