namespace MeetSpot.Core.DTOs.Room;

public class UpdateRoomDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int? Floor { get; set; }
    public int? Capacity { get; set; }
    public decimal? PricePerHour { get; set; }
    public string? Type { get; set; }
    public string? ImageUrl { get; set; }
    public List<string>? Equipment { get; set; }
}