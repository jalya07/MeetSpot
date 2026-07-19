using MeetSpot.Core.DTOs.Mentor;

namespace MeetSpot.Core.Interfaces;

public interface IMentorService
{
    Task<List<MentorResponseDto>> GetAllAsync();
    Task<MentorResponseDto> CreateAsync(CreateMentorDto dto);
    Task DeleteAsync(int id);
    Task UpdateAvatarAsync(int id, string avatarUrl);
    Task<MentorRequestResponseDto> CreateRequestAsync(CreateMentorRequestDto dto);
    Task<List<MentorRequestResponseDto>> GetAllRequestsAsync();
    Task ApproveRequestAsync(int id);
    Task RejectRequestAsync(int id);
}