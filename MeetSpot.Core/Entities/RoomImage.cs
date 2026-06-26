namespace MeetSpot.Core.Entities;

public class RoomImage : BaseEntity
{
    public string ImageUrl { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    
    public int RoomId { get; set; }
    public Room Room { get; set; } = null!;
}