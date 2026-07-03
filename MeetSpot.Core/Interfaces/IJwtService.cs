namespace MeetSpot.Core.Interfaces;

public interface IJwtService
{
    string GenerateToken(int userId, string email, string role, string userType, string fullName = "");
    string GenerateRefreshToken();
    string GenerateTwoFactorCode();
    
}