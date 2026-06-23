namespace MeetSpot.Core.DTOs.Auth;

public class LoginResponseDto
{
    public bool RequiresTwoFactor { get; set; }
    public string? Message { get; set; }
}