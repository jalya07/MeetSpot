namespace MeetSpot.Core.DTOs.Review;

public class CreateReviewDto
{
    public string Text { get; set; } = string.Empty;
    public int Rating { get; set; }
}