namespace MeetSpot.Core.Entities;

public class BlogPost : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? Category { get; set; }
    public bool IsPublished { get; set; } = false;

    public int AuthorId { get; set; }
    public User Author { get; set; } = null!;
}