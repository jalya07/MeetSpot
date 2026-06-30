using MeetSpot.Core.DTOs.Review;

namespace MeetSpot.Core.Interfaces;

public interface IReviewService
{
    Task<ReviewResponseDto> CreateAsync(CreateReviewDto dto, int userId);
    Task<List<ReviewResponseDto>> GetFeaturedAsync();
    Task<List<ReviewResponseDto>> GetAllAsync();
    Task ApproveAsync(int id);
    Task SetFeaturedAsync(int id, bool isFeatured);
    Task DeleteAsync(int id);
}