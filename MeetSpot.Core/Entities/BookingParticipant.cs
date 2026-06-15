namespace MeetSpot.Core.Entities;

public class BookingParticipant : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public int BookingId { get; set; }
    public Booking Booking { get; set; } = null!;
}