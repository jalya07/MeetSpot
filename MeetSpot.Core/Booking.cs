namespace MeetSpot.Core.Entities;

public class Booking : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Active;
    public decimal TotalPrice { get; set; }
    public string? QrCode { get; set; }
    public bool IsCheckedIn { get; set; } = false;
    public DateTime? CheckInTime { get; set; }
    public string? CancelReason { get; set; }

    public int RoomId { get; set; }
    public Room Room { get; set; } = null!;

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public ICollection<BookingParticipant> Participants { get; set; } = new List<BookingParticipant>();
}

public enum BookingStatus
{
    Active,
    Cancelled,
    Completed,
    NoShow
}