using System.Net.Http.Headers;
using System.Text.Json;

namespace MeetSpot.Web.Services;

public class UserProfileDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string UserType { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? AvatarUrl { get; set; }
}

public class TokenResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}

public class UserApiService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpClientFactory _httpClientFactory;

    public UserApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _httpClient = httpClientFactory.CreateClient("MeetSpotAPI");
    }
   
    public async Task<UserProfileDto?> GetProfileAsync(string token)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync("api/user/me");
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<ApiResponse<UserProfileDto>>(json, options);

            return result?.Data;
        }
        catch
        {
            return null;
        }
    }

    public async Task<List<UserProfileDto>> GetAllUsersAsync(string token)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync("api/user");
            if (!response.IsSuccessStatusCode) 
                return new();

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<ApiResponse<List<UserProfileDto>>>(json, options);

            return result?.Data ?? new();
        }
        catch
        {
            return new();
        }
    }

    public async Task<bool> UpdateProfileAsync(string token, string firstName, string lastName)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var dto = new { firstName, lastName };
            var json = System.Text.Json.JsonSerializer.Serialize(dto);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync("api/user/me", content);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
    public async Task<string?> UploadAvatarAsync(string token, IFormFile file)
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

            var response = await client.PostAsync("api/user/upload-avatar", content);
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

    public async Task<TokenResponseDto?> RefreshTokenAsync(string refreshToken,string accessToken)
    {
        try
        {
            Console.WriteLine($"=== RefreshToken called: accessToken={accessToken?.Substring(0, 20)}..., refreshToken={refreshToken?.Substring(0, 10)}... ===");
            var dto = new { accessToken = accessToken, refreshToken };
            var json = System.Text.Json.JsonSerializer.Serialize(dto);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/auth/refresh", content);
            if (!response.IsSuccessStatusCode) return null;

            var responseBody = await response.Content.ReadAsStringAsync();
            var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = System.Text.Json.JsonSerializer.Deserialize<ApiResponse<TokenResponseDto>>(responseBody, options);
            return result?.Data;
        }
        catch
        {
            return null;
        }
    }
}