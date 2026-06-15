using MeetSpot.Core.DTOs.Room;

namespace MeetSpot.Core.Interfaces;

public interface IRoomService
{
    Task<List<RoomResponseDto>> GetAllAsync(string? type, int? capacity, int? floor);
    Task<RoomResponseDto> GetByIdAsync(int id);
    Task<RoomResponseDto> CreateAsync(CreateRoomDto dto);
    Task<RoomResponseDto> UpdateAsync(int id, UpdateRoomDto dto);
    Task DeleteAsync(int id);
    Task<List<RoomResponseDto>> GetAvailableRoomsAsync(DateTime startTime, DateTime endTime, int? capacity);
}