using MeetSpot.Core.DTOs.HeroSlide;

namespace MeetSpot.Core.Interfaces;

public interface IHeroSlideService
{
    Task<List<HeroSlideResponseDto>> GetAllAsync();
    Task<List<HeroSlideResponseDto>> GetActiveAsync();
    Task<HeroSlideResponseDto> CreateAsync(CreateHeroSlideDto dto);
    Task<HeroSlideResponseDto> UpdateAsync(int id, UpdateHeroSlideDto dto);
    Task DeleteAsync(int id);
}