using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;

namespace MeetSpot.Web.Services;

public class PromotionDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal DiscountPercent { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
    public string Type { get; set; } = string.Empty;
}

public class PromotionApiService
{
    private readonly HttpClient _httpClient;

    public PromotionApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("MeetSpotAPI");
    }

    public async Task<List<PromotionDto>> GetActiveAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/promotion");
            if (!response.IsSuccessStatusCode) return new();

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<ApiResponse<List<PromotionDto>>>(json, options);
            return result?.Data ?? new();
        }
        catch { return new(); }
    }

    public async Task<List<PromotionDto>> GetAllAsync(string token)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync("api/promotion/all");
            if (!response.IsSuccessStatusCode) return new();

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<ApiResponse<List<PromotionDto>>>(json, options);
            return result?.Data ?? new();
        }
        catch { return new(); }
    }

    public async Task<bool> CreateAsync(string token, string title, string? description,
        decimal discountPercent, DateTime startDate, DateTime endDate, string type)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var dto = new { title, description, discountPercent, startDate, endDate, type };
            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/promotion", content);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<bool> DeleteAsync(string token, int id)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.DeleteAsync($"api/promotion/{id}");
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }
}