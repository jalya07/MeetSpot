using MeetSpot.Core.DTOs.Promotion;

namespace MeetSpot.Core.Interfaces;

public interface IPromotionService
{
    Task<List<PromotionResponseDto>> GetAllAsync();
    Task<List<PromotionResponseDto>> GetActiveAsync();
    Task<PromotionResponseDto> CreateAsync(CreatePromotionDto dto);
    Task DeleteAsync(int id);
}