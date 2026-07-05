namespace MeetSpot.Core.DTOs.HeroSlide;

public class HeroSlideResponseDto
{
    public int Id { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public string? Caption { get; set; }
}

public class CreateHeroSlideDto
{
    public string ImageUrl { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public string? Caption { get; set; }
}

public class UpdateHeroSlideDto
{
    public string? ImageUrl { get; set; }
    public int? DisplayOrder { get; set; }
    public bool? IsActive { get; set; }
    public string? Caption { get; set; }
}