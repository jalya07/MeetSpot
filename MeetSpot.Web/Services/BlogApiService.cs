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
    private readonly IHttpClientFactory _httpClientFactory;

    public BlogApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
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

    public async Task<List<BlogPostDto>> GetAllAsync(string token)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync("api/blog/all");
            if (!response.IsSuccessStatusCode) 
                return new();

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<ApiResponse<List<BlogPostDto>>>(json, options);

            return result?.Data ?? new();
        }
        catch 
        { 
            return new(); 
        }
    }

    public async Task<bool> CreatePostAsync(string token, string title, string content,string? imageUrl, bool isPublished)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var dto = new { title, content, isPublished };
            var json = JsonSerializer.Serialize(dto);
            var content2 = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/blog", content2);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeletePostAsync(string token, int id)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.DeleteAsync($"api/blog/{id}");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<string?> UploadImageAsync(string token, IFormFile file)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("MeetSpotAPI");
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            using var content = new MultipartFormDataContent();
            using var stream = file.OpenReadStream();
            var streamContent = new StreamContent(stream);
            streamContent.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
            content.Add(streamContent, "file", file.FileName);

            var response = await client.PostAsync("api/blog/upload-image", content);
            if (!response.IsSuccessStatusCode) 
                return null;

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<ApiResponse<string>>(json, options);
            return result?.Data;
        }
        catch 
        { 
            return null; 
        }
    }

    public async Task<bool> UpdatePostAsync(string token, int id, string title, string content, string? imageUrl, bool isPublished)
{
    try
    {
        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var dto = new { title, content, imageUrl, isPublished };
        var json = JsonSerializer.Serialize(dto);
        var content2 = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync($"api/blog/{id}", content2);
        return response.IsSuccessStatusCode;
    }
    catch { return false; }
}

    public async Task<BlogPostDto?> GetByIdAdminAsync(string token, int id)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"api/blog/{id}");
            if (!response.IsSuccessStatusCode) 
                return null;

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<ApiResponse<BlogPostDto>>(json, options);
            return result?.Data;
        }
        catch 
        {
            return null;
        }
    }
}