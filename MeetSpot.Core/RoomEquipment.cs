namespace MeetSpot.Core.Entities;

public class RoomEquipment : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public EquipmentType Type { get; set; }
    public int RoomId { get; set; }
    public Room Room { get; set; } = null!;
}

public enum EquipmentType
{
    Projector,
    Whiteboard,
    VideoConferencing,
    TV,
    AirConditioner,
    Other
}