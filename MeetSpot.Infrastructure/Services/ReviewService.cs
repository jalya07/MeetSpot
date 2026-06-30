using Microsoft.EntityFrameworkCore;
using MeetSpot.Core.DTOs.Review;
using MeetSpot.Core.Entities;
using MeetSpot.Core.Exceptions;
using MeetSpot.Core.Interfaces;
using MeetSpot.Infrastructure.Data;

namespace MeetSpot.Infrastructure.Services;

public class ReviewService : IReviewService
{
    private readonly MeetSpotAppDbContext _context;

    public ReviewService(MeetSpotAppDbContext context)
    {
        _context = context;
    }

    public async Task<ReviewResponseDto> CreateAsync(CreateReviewDto dto, int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            throw new NotFoundException("User not found");

        var review = new Review
        {
            Text = dto.Text,
            Rating = dto.Rating,
            UserId = userId,
            IsApproved = false,
            IsFeatured = false
        };

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        return MapToDto(review, user);
    }

    public async Task<List<ReviewResponseDto>> GetFeaturedAsync()
    {
        var reviews = await _context.Reviews
            .Include(r => r.User)
            .Where(r => r.IsApproved && r.IsFeatured)
            .OrderByDescending(r => r.CreatedAt)
            .Take(6)
            .ToListAsync();

        return reviews.Select(r => MapToDto(r, r.User)).ToList();
    }

    public async Task<List<ReviewResponseDto>> GetAllAsync()
    {
        var reviews = await _context.Reviews
            .Include(r => r.User)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return reviews.Select(r => MapToDto(r, r.User)).ToList();
    }

    public async Task ApproveAsync(int id)
    {
        var review = await _context.Reviews.FindAsync(id);
        if (review == null)
            throw new NotFoundException("Review not found");

        review.IsApproved = true;
        review.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task SetFeaturedAsync(int id, bool isFeatured)
    {
        var review = await _context.Reviews.FindAsync(id);
        if (review == null)
            throw new NotFoundException("Review not found");

        review.IsFeatured = isFeatured;
        review.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var review = await _context.Reviews.FindAsync(id);
        if (review == null)
            throw new NotFoundException("Review not found");

        _context.Reviews.Remove(review);
        await _context.SaveChangesAsync();
    }

    private static ReviewResponseDto MapToDto(Review review, User user)
    {
        return new ReviewResponseDto
        {
            Id = review.Id,
            Text = review.Text,
            Rating = review.Rating,
            IsApproved = review.IsApproved,
            IsFeatured = review.IsFeatured,
            UserFirstName = user.FirstName,
            UserLastName = user.LastName,
            CompanyDomain = user.CompanyDomain,
            CreatedAt = review.CreatedAt
        };
    }
}