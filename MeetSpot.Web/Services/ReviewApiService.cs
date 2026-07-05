using System.Text.Json;

namespace MeetSpot.Web.Services;

public class ReviewResponseDto
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public int Rating { get; set; }
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
}