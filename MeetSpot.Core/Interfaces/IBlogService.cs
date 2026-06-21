using MeetSpot.Core.DTOs.Blog;

namespace MeetSpot.Core.Interfaces;

public interface IBlogService
{
    Task<List<BlogPostResponseDto>> GetPublishedAsync();
    Task<List<BlogPostResponseDto>> GetAllAsync();
    Task<BlogPostResponseDto> GetByIdAsync(int id);
    Task<BlogPostResponseDto> CreateAsync(CreateBlogPostDto dto, int authorId);
    Task<BlogPostResponseDto> UpdateAsync(int id, CreateBlogPostDto dto);
    Task DeleteAsync(int id);
}