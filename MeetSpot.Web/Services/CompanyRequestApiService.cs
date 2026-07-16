using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;

namespace MeetSpot.Web.Services;

public class CompanyRequestDto
{
    public int Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string? Industry { get; set; }
    public string? LogoUrl { get; set; }
    public string? Message { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CompanyRequestApiService
{
    private readonly HttpClient _httpClient;

    public CompanyRequestApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("MeetSpotAPI");
    }

    public async Task<bool> SendRequestAsync(string companyName, string domain, 
        string contactEmail, string? industry, string? logoUrl, string? message)
    {
        try
        {
            var dto = new { companyName, domain, contactEmail, industry, logoUrl, message };
            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/company-request", content);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<List<CompanyRequestDto>> GetAllAsync(string token)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync("api/company-request");
            if (!response.IsSuccessStatusCode) return new();

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<ApiResponse<List<CompanyRequestDto>>>(json, options);
            return result?.Data ?? new();
        }
        catch { return new(); }
    }

    public async Task<bool> ApproveAsync(string token, int id)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PostAsync($"api/company-request/{id}/approve", null);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<bool> RejectAsync(string token, int id)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PostAsync($"api/company-request/{id}/reject", null);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }
}