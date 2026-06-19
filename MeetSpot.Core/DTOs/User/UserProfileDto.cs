namespace MeetSpot.Core.DTOs.User;

public class UserProfileDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string UserType { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public DateTime CreatedAt { get; set; }
}