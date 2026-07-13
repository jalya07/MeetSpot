using MeetSpot.Core.DTOs.User;

namespace MeetSpot.Core.Interfaces;

public interface IUserService
{
    Task<UserProfileDto> GetProfileAsync(int userId);
    Task<UserProfileDto> UpdateProfileAsync(int userId, UpdateProfileDto dto);
    Task<List<UserListItemDto>> GetAllUsersAsync();
    Task ToggleUserStatusAsync(int userId);
    Task UpdateAvatarAsync(int userId, string avatarUrl);
}