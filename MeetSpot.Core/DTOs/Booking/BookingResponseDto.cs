namespace MeetSpot.Core.DTOs.Booking;

public class BookingResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? QrCode { get; set; }
    public bool IsCheckedIn { get; set; }
    public int RoomId { get; set; }
    public string RoomName { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string UserFullName { get; set; } = string.Empty;
    public List<string> Participants { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}