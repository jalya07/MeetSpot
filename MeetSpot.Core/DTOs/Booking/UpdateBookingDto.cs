namespace MeetSpot.Core.DTOs.Booking;

public class UpdateBookingDto
{
    public string? Title { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public List<string>? ParticipantEmails { get; set; }
}