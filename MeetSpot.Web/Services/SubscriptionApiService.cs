using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;

namespace MeetSpot.Web.Services;

public class SubscriptionDto
{
    public int Id { get; set; }
    public string Plan { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

public class SubscriptionApiService
{
    private readonly HttpClient _httpClient;

    public SubscriptionApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("MeetSpotAPI");
    }

    public async Task<SubscriptionDto?> GetMySubscriptionAsync(string token)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync("api/subscription/my");
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<ApiResponse<SubscriptionDto>>(json, options);

            return result?.Data;
        }
        catch { return null; }
    }

    public async Task<bool> CreateSubscriptionAsync(string token, string plan)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var dto = new { plan };
            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/subscription", content);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task CancelSubscriptionAsync(string token)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
            await _httpClient.PostAsync("api/subscription/cancel", null);
        }
        catch { }
    }
}