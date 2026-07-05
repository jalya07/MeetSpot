using Microsoft.EntityFrameworkCore;
using MeetSpot.Core.DTOs.HeroSlide;
using MeetSpot.Core.Entities;
using MeetSpot.Core.Exceptions;
using MeetSpot.Core.Interfaces;
using MeetSpot.Infrastructure.Data;

namespace MeetSpot.Infrastructure.Services;

public class HeroSlideService : IHeroSlideService
{
    private readonly MeetSpotAppDbContext _context;

    public HeroSlideService(MeetSpotAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<HeroSlideResponseDto>> GetAllAsync()
    {
        return await _context.HeroSlides
            .OrderBy(s => s.DisplayOrder)
            .Select(s => MapToDto(s))
            .ToListAsync();
    }

    public async Task<List<HeroSlideResponseDto>> GetActiveAsync()
    {
        return await _context.HeroSlides
            .Where(s => s.IsActive)
            .OrderBy(s => s.DisplayOrder)
            .Select(s => MapToDto(s))
            .ToListAsync();
    }

    public async Task<HeroSlideResponseDto> CreateAsync(CreateHeroSlideDto dto)
    {
        var slide = new HeroSlide
        {
            ImageUrl = dto.ImageUrl,
            DisplayOrder = dto.DisplayOrder,
            Caption = dto.Caption,
            IsActive = true
        };

        _context.HeroSlides.Add(slide);
        await _context.SaveChangesAsync();

        return MapToDto(slide);
    }

    public async Task<HeroSlideResponseDto> UpdateAsync(int id, UpdateHeroSlideDto dto)
    {
        var slide = await _context.HeroSlides.FindAsync(id);
        if (slide == null)
            throw new NotFoundException("Slide not found");

        if (dto.ImageUrl != null) slide.ImageUrl = dto.ImageUrl;
        if (dto.DisplayOrder.HasValue) slide.DisplayOrder = dto.DisplayOrder.Value;
        if (dto.IsActive.HasValue) slide.IsActive = dto.IsActive.Value;
        if (dto.Caption != null) slide.Caption = dto.Caption;
        slide.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return MapToDto(slide);
    }

    public async Task DeleteAsync(int id)
    {
        var slide = await _context.HeroSlides.FindAsync(id);
        if (slide == null)
            throw new NotFoundException("Slide not found");

        _context.HeroSlides.Remove(slide);
        await _context.SaveChangesAsync();
    }

    private static HeroSlideResponseDto MapToDto(HeroSlide slide) => new()
    {
        Id = slide.Id,
        ImageUrl = slide.ImageUrl,
        DisplayOrder = slide.DisplayOrder,
        IsActive = slide.IsActive,
        Caption = slide.Caption
    };
}