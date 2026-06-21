namespace MeetSpot.Core.DTOs.Blog;

public class CreateBlogPostDto
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? Category { get; set; }
    public bool IsPublished { get; set; } = false;
}