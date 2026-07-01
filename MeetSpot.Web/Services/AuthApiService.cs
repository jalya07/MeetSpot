using System.Text;
using System.Text.Json;

namespace MeetSpot.Web.Services;

public class RegisterRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string UserType { get; set; } = "Regular";
}

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class VerifyTwoFactorRequest
{
    public string Email { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}

public class AuthResponseData
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public bool RequiresTwoFactor { get; set; }
    public string? Message { get; set; }
}

public class AuthApiService
{
    private readonly HttpClient _httpClient;

    public AuthApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("MeetSpotAPI");
    }

    public async Task<(bool Success, List<string> Errors)> RegisterAsync(RegisterRequest request)
    {
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("api/auth/register", content);
        var responseBody = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var result = JsonSerializer.Deserialize<ApiResponse<object>>(responseBody, options);

        if (response.IsSuccessStatusCode)
        {
            return (true, new List<string>());
        }

        return (false, result?.Errors ?? new List<string> { "Registration failed" });
    }

    public async Task<(bool Success, bool RequiresTwoFactor, List<string> Errors)> LoginAsync(LoginRequest request)
    {
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("api/auth/login", content);
        var responseBody = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        if (response.IsSuccessStatusCode)
        {
            var result = JsonSerializer.Deserialize<ApiResponse<AuthResponseData>>(responseBody, options);
            return (true, result?.Data?.RequiresTwoFactor ?? false, new List<string>());
        }

        var errorResult = JsonSerializer.Deserialize<ApiResponse<object>>(responseBody, options);
        return (false, false, errorResult?.Errors ?? new List<string> { "Login failed" });
    }

    public async Task<(bool Success, string? AccessToken, string? RefreshToken, List<string> Errors)> VerifyTwoFactorAsync(VerifyTwoFactorRequest request)
    {
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("api/auth/verify-2fa", content);
        var responseBody = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        if (response.IsSuccessStatusCode)
        {
            var result = JsonSerializer.Deserialize<ApiResponse<AuthResponseData>>(responseBody, options);
            return (true, result?.Data?.AccessToken, result?.Data?.RefreshToken, new List<string>());
        }

        var errorResult = JsonSerializer.Deserialize<ApiResponse<object>>(responseBody, options);
        return (false, null, null, errorResult?.Errors ?? new List<string> { "Verification failed" });
    }
}