using System.Text.Json;

namespace MeetSpot.Web.Services;

public class CompanyResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string? Industry { get; set; }
    public bool IsActive { get; set; }
}

public class CompanyApiService
{
    private readonly HttpClient _httpClient;

    public CompanyApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("MeetSpotAPI");
    }

    public async Task<List<CompanyResponseDto>> GetAllCompaniesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/company");
            if (!response.IsSuccessStatusCode) return new();

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<ApiResponse<List<CompanyResponseDto>>>(json, options);

            return result?.Data ?? new();
        }
        catch
        {
            return new();
        }
    }
    
    public async Task<bool> CreateCompanyAsync(string token, string name, string domain, string? industry, string? logoUrl)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var dto = new { name, domain, industry, logoUrl };
            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/company", content);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<bool> DeleteCompanyAsync(string token, int id)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.DeleteAsync($"api/company/{id}");
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }
    
}