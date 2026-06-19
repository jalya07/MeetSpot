using MeetSpot.Core.DTOs.User;

namespace MeetSpot.Core.Interfaces;

public interface IUserService
{
    Task<UserProfileDto> GetProfileAsync(int userId);
    Task<UserProfileDto> UpdateProfileAsync(int userId, UpdateProfileDto dto);
}