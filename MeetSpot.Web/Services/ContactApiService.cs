using System.Text;
using System.Text.Json;

namespace MeetSpot.Web.Services;

public class ContactMessageDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ContactApiService
{
    private readonly HttpClient _httpClient;

    public ContactApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("MeetSpotAPI");
    }

    public async Task<bool> SendMessageAsync(string firstName, string lastName, string email, string subject, string message)
    {
        try
        {
           var dto = new
            {
                fullName = $"{firstName} {lastName}",
                email,
                subject,
                message
            };

            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/contact", content);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<List<ContactMessageDto>> GetAllMessagesAsync(string token)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync("api/contact");
            if (!response.IsSuccessStatusCode) return new();

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<ApiResponse<List<ContactMessageDto>>>(json, options);

            return result?.Data ?? new();
        }
        catch
        {
            return new();
        }
    }

}