using MeetSpot.Core.DTOs.Promotion;
using MeetSpot.Core.Entities;
using MeetSpot.Core.Interfaces;
using MeetSpot.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MeetSpot.Infrastructure.Services;

public class PromotionService : IPromotionService
{
    private readonly MeetSpotAppDbContext _context;

    public PromotionService(MeetSpotAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<PromotionResponseDto>> GetAllAsync()
    {
        var promotions = await _context.Promotions
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
        return promotions.Select(MapToDto).ToList();
    }

    public async Task<List<PromotionResponseDto>> GetActiveAsync()
    {
        var now = DateTime.UtcNow;
        var promotions = await _context.Promotions
            .Where(p => p.IsActive && p.StartDate <= now && p.EndDate >= now)
            .OrderByDescending(p => p.DiscountPercent)
            .ToListAsync();
        return promotions.Select(MapToDto).ToList();
    }

    public async Task<PromotionResponseDto> CreateAsync(CreatePromotionDto dto)
    {
        var promotion = new Promotion
        {
            Title = dto.Title,
            Description = dto.Description,
            DiscountPercent = dto.DiscountPercent,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            IsActive = true,
            Type = Enum.Parse<PromotionType>(dto.Type, true)
        };

        _context.Promotions.Add(promotion);
        await _context.SaveChangesAsync();
        return MapToDto(promotion);
    }

    public async Task DeleteAsync(int id)
    {
        var promotion = await _context.Promotions.FindAsync(id);
        if (promotion != null)
        {
            _context.Promotions.Remove(promotion);
            await _context.SaveChangesAsync();
        }
    }

    private PromotionResponseDto MapToDto(Promotion p) => new()
    {
        Id = p.Id,
        Title = p.Title,
        Description = p.Description,
        DiscountPercent = p.DiscountPercent,
        StartDate = p.StartDate,
        EndDate = p.EndDate,
        IsActive = p.IsActive,
        Type = p.Type.ToString()
    };
}