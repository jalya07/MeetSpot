using Microsoft.EntityFrameworkCore;
using MeetSpot.Core.DTOs.Room;
using MeetSpot.Core.Entities;
using MeetSpot.Core.Exceptions;
using MeetSpot.Core.Interfaces;
using MeetSpot.Infrastructure.Data;

namespace MeetSpot.Infrastructure.Services;

public class RoomService : IRoomService
{
    private readonly MeetSpotAppDbContext _context;
    private readonly ICacheService _cacheService;

    public RoomService(MeetSpotAppDbContext context, ICacheService cacheService)
    {
        _context = context;
        _cacheService = cacheService;
    }

   public async Task<List<RoomResponseDto>> GetAllAsync(string? type, int? capacity, int? floor)
    {
        var cacheKey = $"rooms_{type}_{capacity}_{floor}";

        var cached = await _cacheService.GetAsync<List<RoomResponseDto>>(cacheKey);
        if (cached != null) 
            return cached;

        var query = _context.Rooms
            .Include(r => r.Equipment)
            .Where(r => r.IsActive);

        if (!string.IsNullOrEmpty(type) && Enum.TryParse<RoomType>(type, true, out var roomType))
            query = query.Where(r => r.Type == roomType);

        if (capacity.HasValue)
            query = query.Where(r => r.Capacity >= capacity.Value);

        if (floor.HasValue)
            query = query.Where(r => r.Floor == floor.Value);

        var result = await query.Select(r => ToDto(r)).ToListAsync();
        
        await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));
        
        return result;

    }

    public async Task<RoomResponseDto> GetByIdAsync(int id)
    {
        var room = await _context.Rooms
            .Include(r => r.Equipment)
            .FirstOrDefaultAsync(r => r.Id == id && r.IsActive);

        if (room == null)
            throw new NotFoundException("Room not found");

        return ToDto(room);
    }

    public async Task<RoomResponseDto> CreateAsync(CreateRoomDto dto)
    {
        var room = new Room
        {
            Name = dto.Name,
            Description = dto.Description,
            Floor = dto.Floor,
            Capacity = dto.Capacity,
            PricePerHour = dto.PricePerHour,
            Type = Enum.Parse<RoomType>(dto.Type, true),
            ImageUrl = dto.ImageUrl,
            IsActive = true
        };

        foreach (var eq in dto.Equipment)
        {
            room.Equipment.Add(new RoomEquipment
            {
                Name = eq,
                Type = EquipmentType.Other
            });
        }

        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();
        await _cacheService.RemoveAsync($"rooms___");

        return ToDto(room);
    }

    public async Task<RoomResponseDto> UpdateAsync(int id, UpdateRoomDto dto)
    {
        var room = await _context.Rooms
            .Include(r => r.Equipment)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (room == null)
            throw new NotFoundException("Room not found");

        if (dto.Name != null) room.Name = dto.Name;
        if (dto.Description != null) room.Description = dto.Description;
        if (dto.Floor.HasValue) room.Floor = dto.Floor.Value;
        if (dto.Capacity.HasValue) room.Capacity = dto.Capacity.Value;
        if (dto.PricePerHour.HasValue) room.PricePerHour = dto.PricePerHour.Value;
        if (dto.ImageUrl != null) room.ImageUrl = dto.ImageUrl;
        if (dto.Type != null) room.Type = Enum.Parse<RoomType>(dto.Type, true);
        room.UpdatedAt = DateTime.UtcNow;

        if (dto.Equipment != null)
        {
            _context.RoomEquipments.RemoveRange(room.Equipment);
            foreach (var eq in dto.Equipment)
            {
                room.Equipment.Add(new RoomEquipment
                {
                    Name = eq,
                    Type = EquipmentType.Other
                });
            }
        }

        await _context.SaveChangesAsync();
        await _cacheService.RemoveAsync($"rooms___");
        return ToDto(room);
    }

    public async Task DeleteAsync(int id)
    {
        var room = await _context.Rooms.FindAsync(id);

        if (room == null)
            throw new NotFoundException("Room not found");

        room.IsActive = false;
        room.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        await _cacheService.RemoveAsync($"rooms___");
    }

    public async Task<List<RoomResponseDto>> GetAvailableRoomsAsync(DateTime startTime, DateTime endTime, int? capacity)
    {
        var bookedRoomIds = await _context.Bookings
            .Where(b => b.Status == BookingStatus.Active
                && b.StartTime < endTime
                && b.EndTime > startTime)
            .Select(b => b.RoomId)
            .ToListAsync();

        var query = _context.Rooms
            .Include(r => r.Equipment)
            .Where(r => r.IsActive && !bookedRoomIds.Contains(r.Id));

        if (capacity.HasValue)
            query = query.Where(r => r.Capacity >= capacity.Value);

        return await query.Select(r => ToDto(r)).ToListAsync();
    }

    private static RoomResponseDto ToDto(Room r) => new()
    {
        Id = r.Id,
        Name = r.Name,
        Description = r.Description,
        Floor = r.Floor,
        Capacity = r.Capacity,
        PricePerHour = r.PricePerHour,
        Type = r.Type.ToString(),
        IsActive = r.IsActive,
        ImageUrl = r.ImageUrl,
        Equipment = r.Equipment.Select(e => e.Name).ToList(),
        CreatedAt = r.CreatedAt
    };
}