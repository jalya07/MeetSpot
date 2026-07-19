using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;

namespace MeetSpot.Web.Services;

public class MentorDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string Role { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
    public List<string> Skills { get; set; } = new();
    public decimal Rating { get; set; }
    public int SessionsCount { get; set; }
}

public class MentorRequestDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? Skills { get; set; }
    public string? LinkedInUrl { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class MentorApiService
{
    private readonly HttpClient _httpClient;

    public MentorApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("MeetSpotAPI");
    }

    public async Task<List<MentorDto>> GetAllAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/mentor");
            if (!response.IsSuccessStatusCode) return new();
            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<ApiResponse<List<MentorDto>>>(json, options);
            return result?.Data ?? new();
        }
        catch { return new(); }
    }

    public async Task<bool> CreateAsync(string token, string firstName, string lastName,
        string role, string company, string? bio, List<string> skills, decimal rating, int sessions)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
            var dto = new { firstName, lastName, role, company, bio, skills, rating, sessionsCount = sessions };
            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/mentor", content);
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
            var response = await _httpClient.DeleteAsync($"api/mentor/{id}");
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<bool> SendRequestAsync(string firstName, string lastName, string email,
        string role, string company, string? bio, string? skills, string? linkedInUrl)
    {
        try
        {
            var dto = new { firstName, lastName, email, role, company, bio, skills, linkedInUrl };
            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/mentor/request", content);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<List<MentorRequestDto>> GetAllRequestsAsync(string token)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync("api/mentor/requests");
            if (!response.IsSuccessStatusCode) return new();
            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<ApiResponse<List<MentorRequestDto>>>(json, options);
            return result?.Data ?? new();
        }
        catch { return new(); }
    }

    public async Task<bool> ApproveRequestAsync(string token, int id)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PostAsync($"api/mentor/requests/{id}/approve", null);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<bool> RejectRequestAsync(string token, int id)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PostAsync($"api/mentor/requests/{id}/reject", null);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }
}