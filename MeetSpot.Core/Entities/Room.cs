namespace MeetSpot.Core.Entities;

public class Room : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Floor { get; set; }
    public int Capacity { get; set; }
    public decimal PricePerHour { get; set; }
    public RoomType Type { get; set; }
    public bool IsActive { get; set; } = true;
    public string? ImageUrl { get; set; }

    public ICollection<RoomEquipment> Equipment { get; set; } = new List<RoomEquipment>();
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public ICollection<RoomImage> Images { get; set; } = new List<RoomImage>();
}

public enum RoomType
{
    Standard,
    Vip
}