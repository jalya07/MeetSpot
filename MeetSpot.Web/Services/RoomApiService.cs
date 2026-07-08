using System.Text.Json;

namespace MeetSpot.Web.Services;

public class RoomResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Floor { get; set; }
    public int Capacity { get; set; }
    public decimal PricePerHour { get; set; }
    public string Type { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public List<string> ImageUrls { get; set; } = new();
    public List<string> Equipment { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public List<string> Errors { get; set; } = new();
    public T? Data { get; set; }
    public int StatusCode { get; set; }
}

public class RoomApiService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpClientFactory _httpClientFactory;

    public RoomApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("MeetSpotAPI");
        _httpClientFactory = httpClientFactory;
    }

    public async Task<List<RoomResponseDto>> GetAllRoomsAsync()
    {
        var response = await _httpClient.GetAsync("api/room");
        var json = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var result = JsonSerializer.Deserialize<ApiResponse<List<RoomResponseDto>>>(json, options);

        return result?.Data ?? new List<RoomResponseDto>();
    }
    public async Task<RoomResponseDto> GetRoomByIdAsync(int id)
    {
        var response = await _httpClient.GetAsync($"api/room/{id}");
        var json = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var result = JsonSerializer.Deserialize<ApiResponse<RoomResponseDto>>(json, options);

        return result?.Data ?? throw new Exception("Room not found");
    }
    public async Task DeleteRoomAsync(string token, int id)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            await _httpClient.DeleteAsync($"api/room/{id}");
        }
        catch { }
    }
    public async Task<bool> UpdateRoomAsync(string token, int id, object dto)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"api/room/{id}", content);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }
    public async Task<bool> CreateRoomAsync(string token, string name, string description, int floor, int capacity, decimal pricePerHour, string type, List<string> imageUrls, List<string> equipment)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var dto = new { name, description, floor, capacity, pricePerHour, type, imageUrls, equipment };
            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/room", content);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<string?> UploadRoomImageAsync(string token, IFormFile file)
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

            var response = await client.PostAsync("api/room/upload-image", content);
            if (!response.IsSuccessStatusCode) return null;

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
}