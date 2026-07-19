using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using MeetSpot.Core.DTOs.Booking;
using MeetSpot.Core.Entities;
using MeetSpot.Core.Exceptions;
using MeetSpot.Core.Interfaces;
using MeetSpot.Infrastructure.Data;
using MeetSpot.Infrastructure.Services;
using AutoMapper;

namespace MeetSpot.Tests.Services;

public class BookingServiceTests
{
    private MeetSpotAppDbContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<MeetSpotAppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new MeetSpotAppDbContext(options);
    }

    private BookingService CreateService(MeetSpotAppDbContext context)
    {
        var emailServiceMock = new Mock<IEmailService>();
        var notificationServiceMock = new Mock<IBookingNotificationService>();
        var mapperMock = new Mock<IMapper>();
        return new BookingService(context, emailServiceMock.Object, notificationServiceMock.Object, mapperMock.Object);
    }

    // Вспомогательный метод — следующий рабочий день в 10:00
    private DateTime NextWorkDay(int addDays = 0) =>
        DateTime.UtcNow.AddDays(addDays).Date.AddHours(10);

    [Fact]
    public async Task CreateAsync_ShouldThrowConflictException_WhenRoomAlreadyBooked()
    {
        var context = GetInMemoryContext();
        var room = new Room { Name = "Room A", Capacity = 5, PricePerHour = 10m, IsActive = true };
        var user = new User { Email = "test@test.com", FirstName = "Test", LastName = "User", Type = UserType.Premium, Balance = 100m };
        context.Rooms.Add(room);
        context.Users.Add(user);
        await context.SaveChangesAsync();

        context.Bookings.Add(new Booking
        {
            RoomId = room.Id,
            UserId = user.Id,
            StartTime = NextWorkDay(1),
            EndTime = NextWorkDay(1).AddHours(2),
            Status = BookingStatus.Active,
            Title = "Existing"
        });
        await context.SaveChangesAsync();

        var bookingService = CreateService(context);

        var dto = new CreateBookingDto
        {
            Title = "New Booking",
            RoomId = room.Id,
            StartTime = NextWorkDay(1).AddMinutes(30),
            EndTime = NextWorkDay(1).AddHours(2).AddMinutes(30),
            ParticipantEmails = new List<string>()
        };

        await Assert.ThrowsAsync<ConflictException>(() => bookingService.CreateAsync(dto, user.Id));
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowNotFoundException_WhenRoomDoesNotExist()
    {
        var context = GetInMemoryContext();
        var user = new User { Email = "test2@test.com", FirstName = "Test", LastName = "User", Type = UserType.Premium, Balance = 100m };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var bookingService = CreateService(context);

        var dto = new CreateBookingDto
        {
            Title = "Test",
            RoomId = 999,
            StartTime = NextWorkDay(1),
            EndTime = NextWorkDay(1).AddHours(2),
            ParticipantEmails = new List<string>()
        };

        await Assert.ThrowsAsync<NotFoundException>(() => bookingService.CreateAsync(dto, user.Id));
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowBadRequestException_WhenRegularUserBooksTooSoon()
    {
        var context = GetInMemoryContext();
        var room = new Room { Name = "Room B", Capacity = 5, PricePerHour = 10m, IsActive = true };
        var user = new User { Email = "regular@test.com", FirstName = "Test", LastName = "User", Type = UserType.Regular, Balance = 1000m };
        context.Rooms.Add(room);
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var bookingService = CreateService(context);

        var dto = new CreateBookingDto
        {
            Title = "Too soon",
            RoomId = room.Id,
            StartTime = NextWorkDay(1),
            EndTime = NextWorkDay(1).AddHours(2),
            ParticipantEmails = new List<string>()
        };

        await Assert.ThrowsAsync<BadRequestException>(() => bookingService.CreateAsync(dto, user.Id));
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowBadRequestException_WhenInsufficientBalance()
    {
        var context = GetInMemoryContext();
        var room = new Room { Name = "Room C", Capacity = 5, PricePerHour = 100m, IsActive = true };
        var user = new User { Email = "poor@test.com", FirstName = "Test", LastName = "User", Type = UserType.Premium, Balance = 5m, Role = UserRole.User };
        context.Rooms.Add(room);
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var bookingService = CreateService(context);

        var dto = new CreateBookingDto
        {
            Title = "Expensive booking",
            RoomId = room.Id,
            StartTime = NextWorkDay(1),
            EndTime = NextWorkDay(1).AddHours(2),
            ParticipantEmails = new List<string>()
        };

        await Assert.ThrowsAsync<BadRequestException>(() => bookingService.CreateAsync(dto, user.Id));
    }
}