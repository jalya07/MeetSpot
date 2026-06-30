namespace MeetSpot.Core.DTOs.Review;

public class ReviewResponseDto
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public int Rating { get; set; }
    public bool IsApproved { get; set; }
    public bool IsFeatured { get; set; }
    public string UserFirstName { get; set; } = string.Empty;
    public string UserLastName { get; set; } = string.Empty;
    public string? CompanyDomain { get; set; }
    public DateTime CreatedAt { get; set; }
}