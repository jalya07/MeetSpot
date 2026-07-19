namespace MeetSpot.Core.DTOs.Mentor;

public class MentorResponseDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string Role { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
    public List<string> Skills { get; set; } = new();
    public decimal Rating { get; set; }
    public int SessionsCount { get; set; }
    public bool IsActive { get; set; }
}