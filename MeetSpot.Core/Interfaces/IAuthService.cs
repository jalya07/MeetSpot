using MeetSpot.Core.DTOs.Auth;

namespace MeetSpot.Core.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
    Task<LoginResponseDto> LoginAsync(LoginDto dto);
    Task<AuthResponseDto> VerifyTwoFactorAsync(VerifyTwoFactorDto dto);
    Task<AuthResponseDto> RefreshTokenAsync(string accessToken, string refreshToken);
    Task ConfirmEmailAsync(string email, string token);
    Task ForgotPasswordAsync(string email);
    Task ResetPasswordAsync(string token, string newPassword);
}