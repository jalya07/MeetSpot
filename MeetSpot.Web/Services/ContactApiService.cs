using System.Text;
using System.Text.Json;

namespace MeetSpot.Web.Services;

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
                name = $"{firstName} {lastName}",
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
}