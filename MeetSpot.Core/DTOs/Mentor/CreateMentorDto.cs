namespace MeetSpot.Core.DTOs.Mentor;

public class CreateMentorDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public List<string> Skills { get; set; } = new();
    public decimal Rating { get; set; } = 5;
    public int SessionsCount { get; set; } = 0;
}