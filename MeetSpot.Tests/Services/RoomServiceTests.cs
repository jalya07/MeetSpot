using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using AutoMapper;
using MeetSpot.Core.DTOs.Room;
using MeetSpot.Core.Entities;
using MeetSpot.Core.Exceptions;
using MeetSpot.Core.Interfaces;
using MeetSpot.Infrastructure.Data;
using MeetSpot.Infrastructure.Services;
using MeetSpot.Core.Mappings;

namespace MeetSpot.Tests.Services;

public class RoomServiceTests
{
    private MeetSpotAppDbContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<MeetSpotAppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new MeetSpotAppDbContext(options);
    }

    private IMapper GetRealMapper()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        return config.CreateMapper();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowNotFoundException_WhenRoomDoesNotExist()
    {
        var context = GetInMemoryContext();
        var cacheMock = new Mock<ICacheService>();
        var mapper = GetRealMapper();
        var roomService = new RoomService(context, cacheMock.Object, mapper);

        await Assert.ThrowsAsync<NotFoundException>(() => roomService.GetByIdAsync(999));
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateRoom_WithCorrectData()
    {
        var context = GetInMemoryContext();
        var cacheMock = new Mock<ICacheService>();
        var mapper = GetRealMapper();
        var roomService = new RoomService(context, cacheMock.Object, mapper);

        var dto = new CreateRoomDto
        {
            Name = "Test Room",
            Description = "A test room",
            Floor = 2,
            Capacity = 8,
            PricePerHour = 20m,
            Type = "Standard",
            Equipment = new List<string> { "Projector" }
        };

        var result = await roomService.CreateAsync(dto);

        Assert.Equal("Test Room", result.Name);
        Assert.Equal(8, result.Capacity);
        Assert.Equal(20m, result.PricePerHour);
        Assert.Equal("Standard", result.Type);
        Assert.Contains("Projector", result.Equipment);
    }

    [Fact]
    public async Task GetAvailableRoomsAsync_ShouldExcludeBookedRooms()
    {
        var context = GetInMemoryContext();
        var cacheMock = new Mock<ICacheService>();
        var mapper = GetRealMapper();

        var room1 = new Room { Name = "Room A", Capacity = 5, PricePerHour = 10m, IsActive = true };
        var room2 = new Room { Name = "Room B", Capacity = 5, PricePerHour = 10m, IsActive = true };
        var user = new User { Email = "u@test.com", FirstName = "U", LastName = "Test", Type = UserType.Corporate };
        context.Rooms.AddRange(room1, room2);
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var startTime = DateTime.UtcNow.AddDays(1);
        var endTime = startTime.AddHours(2);

        context.Bookings.Add(new Booking
        {
            RoomId = room1.Id,
            UserId = user.Id,
            StartTime = startTime,
            EndTime = endTime,
            Status = BookingStatus.Active,
            Title = "Booked"
        });
        await context.SaveChangesAsync();

        var roomService = new RoomService(context, cacheMock.Object, mapper);

        var available = await roomService.GetAvailableRoomsAsync(startTime, endTime, null);

        Assert.DoesNotContain(available, r => r.Name == "Room A");
        Assert.Contains(available, r => r.Name == "Room B");
    }
}