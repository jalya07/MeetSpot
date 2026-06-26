namespace MeetSpot.Core.DTOs.Room;

public class CreateRoomDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Floor { get; set; }
    public int Capacity { get; set; }
    public decimal PricePerHour { get; set; }
    public string Type { get; set; } = "Standard";
    public List<string> Equipment { get; set; } = new();
    public List<string> ImageUrls { get; set; } = new();
}