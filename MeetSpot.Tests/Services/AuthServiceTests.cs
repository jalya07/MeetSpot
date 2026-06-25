using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using MeetSpot.Core.DTOs.Auth;
using MeetSpot.Core.Entities;
using MeetSpot.Core.Exceptions;
using MeetSpot.Core.Interfaces;
using MeetSpot.Infrastructure.Data;
using MeetSpot.Infrastructure.Services;

namespace MeetSpot.Tests.Services;

public class AuthServiceTests
{
    private MeetSpotAppDbContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<MeetSpotAppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new MeetSpotAppDbContext(options);
    }

    private AuthService CreateService(MeetSpotAppDbContext context, ICompanyService? companyService = null)
    {
        var jwtServiceMock = new Mock<IJwtService>();
        jwtServiceMock.Setup(j => j.GenerateToken(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns("fake-access-token");
        jwtServiceMock.Setup(j => j.GenerateRefreshToken()).Returns("fake-refresh-token");
        jwtServiceMock.Setup(j => j.GenerateTwoFactorCode()).Returns("123456");

        var emailServiceMock = new Mock<IEmailService>();
        var companyServiceMock = companyService ?? new Mock<ICompanyService>().Object;

        return new AuthService(context, jwtServiceMock.Object, emailServiceMock.Object, companyServiceMock);
    }

    [Fact]
    public async Task RegisterAsync_ShouldThrowConflictException_WhenEmailAlreadyExists()
    {
        var context = GetInMemoryContext();
        context.Users.Add(new User
        {
            Email = "existing@test.com",
            FirstName = "Existing",
            LastName = "User",
            PasswordHash = "hash",
            Type = UserType.Regular
        });
        await context.SaveChangesAsync();

        var authService = CreateService(context);

        var dto = new RegisterDto
        {
            FirstName = "New",
            LastName = "User",
            Email = "existing@test.com",
            Password = "Password123!",
            UserType = "Regular"
        };

        await Assert.ThrowsAsync<ConflictException>(() => authService.RegisterAsync(dto));
    }

    [Fact]
    public async Task LoginAsync_ShouldThrowUnauthorizedException_WhenPasswordIsWrong()
    {
        var context = GetInMemoryContext();
        context.Users.Add(new User
        {
            Email = "user@test.com",
            FirstName = "Test",
            LastName = "User",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword123!"),
            Type = UserType.Regular,
            IsActive = true,
            IsEmailConfirmed = true
        });
        await context.SaveChangesAsync();

        var authService = CreateService(context);

        var dto = new LoginDto
        {
            Email = "user@test.com",
            Password = "WrongPassword123!"
        };

        await Assert.ThrowsAsync<UnauthorizedException>(() => authService.LoginAsync(dto));
    }

    [Fact]
    public async Task LoginAsync_ShouldThrowUnauthorizedException_WhenEmailNotConfirmed()
    {
        var context = GetInMemoryContext();
        context.Users.Add(new User
        {
            Email = "unconfirmed@test.com",
            FirstName = "Test",
            LastName = "User",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
            Type = UserType.Regular,
            IsActive = true,
            IsEmailConfirmed = false
        });
        await context.SaveChangesAsync();

        var authService = CreateService(context);

        var dto = new LoginDto
        {
            Email = "unconfirmed@test.com",
            Password = "Password123!"
        };

        await Assert.ThrowsAsync<UnauthorizedException>(() => authService.LoginAsync(dto));
    }
}