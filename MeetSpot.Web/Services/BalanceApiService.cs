using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;

namespace MeetSpot.Web.Services;

public class TransactionDto
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal BalanceAfter { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class BalanceApiService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpClientFactory _httpClientFactory;

    public BalanceApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _httpClient = _httpClientFactory.CreateClient("MeetSpotAPI");
    }

    public async Task<List<TransactionDto>> GetTransactionsAsync(string token)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync("api/balance/transactions");
            if (!response.IsSuccessStatusCode) return new();

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<ApiResponse<List<TransactionDto>>>(json, options);

            return result?.Data ?? new();
        }
        catch { return new(); }
    }

    public async Task<bool> TopUpAsync(string token, decimal amount, string cardNumber, string cardHolderName, string expiryDate, string cvv)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("MeetSpotAPI");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var dto = new { amount, cardNumber, cardHolderName, expiryDate, cvv };
            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/balance/topup", content);

            var responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"=== TopUp response: {response.StatusCode} - {responseBody} ===");

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"=== TopUp error: {ex.Message} ===");
            return false;
        }
    }
}