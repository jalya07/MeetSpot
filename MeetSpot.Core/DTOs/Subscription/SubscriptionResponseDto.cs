namespace MeetSpot.Core.DTOs.Subscription;

public class SubscriptionResponseDto
{
    public int Id { get; set; }
    public string Plan { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
    public decimal Price { get; set; }
}