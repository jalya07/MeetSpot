namespace MeetSpot.Core.DTOs.Mentor;

public class CreateMentorRequestDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? Skills { get; set; }
    public string? LinkedInUrl { get; set; }
}