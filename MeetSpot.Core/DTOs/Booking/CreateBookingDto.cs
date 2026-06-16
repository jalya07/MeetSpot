namespace MeetSpot.Core.DTOs.Booking;

public class CreateBookingDto
{
    public string Title { get; set; } = string.Empty;
    public int RoomId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public List<string> ParticipantEmails { get; set; } = new();
}