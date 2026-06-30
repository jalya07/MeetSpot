namespace MeetSpot.Core.Entities;

public class Review : BaseEntity
{
    public string Text { get; set; } = string.Empty;
    public int Rating { get; set; }
    public bool IsApproved { get; set; }
    public bool IsFeatured { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;
}