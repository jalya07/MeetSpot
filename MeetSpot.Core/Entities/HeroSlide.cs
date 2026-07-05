namespace MeetSpot.Core.Entities;

public class HeroSlide : BaseEntity
{
    public string ImageUrl { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Caption { get; set; }
}