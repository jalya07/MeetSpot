using Microsoft.EntityFrameworkCore;
using MeetSpot.Core.DTOs.Auth;
using MeetSpot.Core.Entities;
using MeetSpot.Core.Interfaces;
using MeetSpot.Infrastructure.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MeetSpot.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly MeetSpotAppDbContext _context;
    private readonly IJwtService _jwtService;
    private readonly IEmailService _emailService;

    public AuthService(MeetSpotAppDbContext context, IJwtService jwtService, IEmailService emailService)
    {
        _context = context;
        _jwtService = jwtService;
        _emailService = emailService;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (existingUser != null)
            throw new Exception("User with this email already exists");

        var userType = Enum.Parse<UserType>(dto.UserType, true);

        var user = new User
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = UserRole.User,
            Type = userType,
            CompanyDomain = dto.CompanyDomain,
            IsEmailConfirmed = false
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var accessToken = _jwtService.GenerateToken(
            user.Id, user.Email, user.Role.ToString(), user.Type.ToString());
        var refreshToken = _jwtService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        await _context.SaveChangesAsync();
        var confirmToken = Guid.NewGuid().ToString();
        user.EmailConfirmationToken = confirmToken;
        user.EmailConfirmationTokenExpiry = DateTime.UtcNow.AddHours(24);
        await _context.SaveChangesAsync();

        var confirmLink = $"http://localhost:5249/api/auth/confirm-email?token={confirmToken}&email={user.Email}";
        await _emailService.SendEmailConfirmationAsync(user.Email, $"{user.FirstName} {user.LastName}", confirmLink);
        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Email = user.Email,
            FullName = $"{user.FirstName} {user.LastName}",
            Role = user.Role.ToString(),
            UserType = user.Type.ToString()
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new Exception("Invalid email or password");

        if (!user.IsActive)
            throw new Exception("Account is disabled");
        
        if (!user.IsEmailConfirmed)
            throw new Exception("Please confirm your email before logging in");

        var accessToken = _jwtService.GenerateToken(
            user.Id, user.Email, user.Role.ToString(), user.Type.ToString());
        var refreshToken = _jwtService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        await _context.SaveChangesAsync();

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Email = user.Email,
            FullName = $"{user.FirstName} {user.LastName}",
            Role = user.Role.ToString(),
            UserType = user.Type.ToString()
        };
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(string accessToken, string refreshToken)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(accessToken);
        var email = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email
                && u.RefreshToken == refreshToken
                && u.RefreshTokenExpiry > DateTime.UtcNow);

        if (user == null)
            throw new Exception("Invalid or expired refresh token");

        var newAccessToken = _jwtService.GenerateToken(
            user.Id, user.Email, user.Role.ToString(), user.Type.ToString());
        var newRefreshToken = _jwtService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        await _context.SaveChangesAsync();

        return new AuthResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            Email = user.Email,
            FullName = $"{user.FirstName} {user.LastName}",
            Role = user.Role.ToString(),
            UserType = user.Type.ToString()
        };
    }

    public async Task ConfirmEmailAsync(string email, string token)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
            throw new Exception("User not found");

        if (user.EmailConfirmationToken != token)
            throw new Exception("Invalid confirmation token");

        if (user.EmailConfirmationTokenExpiry < DateTime.UtcNow)
            throw new Exception("Confirmation token has expired");

        user.IsEmailConfirmed = true;
        user.EmailConfirmationToken = null;
        user.EmailConfirmationTokenExpiry = null;
        await _context.SaveChangesAsync();
    }

    public async Task ForgotPasswordAsync(string email)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
            throw new Exception("User not found");

        var resetToken = Guid.NewGuid().ToString();
        user.PasswordResetToken = resetToken;
        user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1);
        await _context.SaveChangesAsync();

        var resetLink = $"http://localhost:5249/api/auth/reset-password?token={resetToken}&email={email}";
        await _emailService.SendPasswordResetAsync(user.Email, $"{user.FirstName} {user.LastName}", resetLink);
    }

    public async Task ResetPasswordAsync(string token, string newPassword)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.PasswordResetToken == token);

        if (user == null)
            throw new Exception("Invalid reset token");

        if (user.PasswordResetTokenExpiry < DateTime.UtcNow)
            throw new Exception("Reset token has expired");

         if (BCrypt.Net.BCrypt.Verify(newPassword, user.PasswordHash))
            throw new Exception("New password cannot be the same as the old password");
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        user.PasswordResetToken = null;
        user.PasswordResetTokenExpiry = null;
       
        await _context.SaveChangesAsync();
    }
}