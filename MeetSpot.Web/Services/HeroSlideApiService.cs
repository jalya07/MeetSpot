using System.Net.Http.Headers;
using System.Text.Json;

namespace MeetSpot.Web.Services;

public class HeroSlideDto
{
    public int Id { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public string? Caption { get; set; }
}

public class HeroSlideApiService
{
    private readonly HttpClient _httpClient;

    public HeroSlideApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("MeetSpotAPI");
    }

    public async Task<List<HeroSlideDto>> GetActiveSlidesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/heroslide");
            if (!response.IsSuccessStatusCode) return new();

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<ApiResponse<List<HeroSlideDto>>>(json, options);

            return result?.Data ?? new();
        }
        catch { return new(); }
    }

    public async Task<List<HeroSlideDto>> GetAllSlidesAsync(string token)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync("api/heroslide/all");
            if (!response.IsSuccessStatusCode) return new();

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<ApiResponse<List<HeroSlideDto>>>(json, options);

            return result?.Data ?? new();
        }
        catch { return new(); }
    }

    public async Task<bool> CreateSlideAsync(string token, string imageUrl, int displayOrder, string? caption)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var dto = new { imageUrl, displayOrder, caption };
            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/heroslide", content);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task DeleteSlideAsync(string token, int id)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
            await _httpClient.DeleteAsync($"api/heroslide/{id}");
        }
        catch { }
    }

    public async Task<string?> UploadImageAsync(string token, MultipartFormDataContent content)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PostAsync("api/heroslide/upload", content);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<ApiResponse<string>>(json, options);

            return result?.Data;
        }
        catch { return null; }
    }
}