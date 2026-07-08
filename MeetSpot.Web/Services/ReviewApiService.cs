using System.Text.Json;

namespace MeetSpot.Web.Services;

public class ReviewResponseDto
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public int Rating { get; set; }
    public bool IsApproved { get; set; }
    public bool IsFeatured { get; set; }
    public string UserFirstName { get; set; } = string.Empty;
    public string UserLastName { get; set; } = string.Empty;
    public string? CompanyDomain { get; set; }
    public DateTime CreatedAt { get; set; }
}


public class ReviewApiService
{
    private readonly HttpClient _httpClient;

    public ReviewApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("MeetSpotAPI");
    }

    public async Task<List<ReviewResponseDto>> GetFeaturedAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/review/featured");
            if (!response.IsSuccessStatusCode) return new();

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<ApiResponse<List<ReviewResponseDto>>>(json, options);

            return result?.Data ?? new();
        }
        catch
        {
            return new();
        }
    }
    public async Task<List<ReviewResponseDto>> GetAllReviewsAsync(string token)
    {
        try
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5249/");
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync("api/review");
            if (!response.IsSuccessStatusCode) 
                return new();

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<ApiResponse<List<ReviewResponseDto>>>(json, options);

            return result?.Data ?? new();
        }
        catch
        {
            return new();
        }
    }
    public async Task ApproveReviewAsync(string token, int id)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            await _httpClient.PostAsync($"api/review/{id}/approve", null);
        }
        catch { }
    }

    public async Task DeleteReviewAsync(string token, int id)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            await _httpClient.DeleteAsync($"api/review/{id}");
        }
        catch { }
    }

    public async Task SetFeaturedAsync(string token, int id, bool isFeatured)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            await _httpClient.PostAsync($"api/review/{id}/featured?isFeatured={isFeatured}", null);
        }
        catch { }
    }

    public async Task<bool> CreateReviewAsync(string token, string text, int rating)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var dto = new { text, rating };
            var json = System.Text.Json.JsonSerializer.Serialize(dto);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/review", content);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }
}