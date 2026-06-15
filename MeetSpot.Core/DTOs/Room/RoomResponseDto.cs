namespace MeetSpot.Core.DTOs.Room;

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
    public string? ImageUrl { get; set; }
    public List<string> Equipment { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}