using System.Text.Json;

namespace MeetSpot.Web.Services;

public class BlogPostDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsPublished { get; set; }
}

public class BlogApiService
{
    private readonly HttpClient _httpClient;

    public BlogApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("MeetSpotAPI");
    }

    public async Task<List<BlogPostDto>> GetPublishedAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/blog");
            if (!response.IsSuccessStatusCode) return new();

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<ApiResponse<List<BlogPostDto>>>(json, options);

            return result?.Data ?? new();
        }
        catch { return new(); }
    }

    public async Task<BlogPostDto?> GetByIdAsync(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/blog/{id}");
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<ApiResponse<BlogPostDto>>(json, options);

            return result?.Data;
        }
        catch { return null; }
    }
}