using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MeetSpot.Core.DTOs.Blog;
using MeetSpot.Core.Entities;
using MeetSpot.Core.Exceptions;
using MeetSpot.Core.Interfaces;
using MeetSpot.Infrastructure.Data;

namespace MeetSpot.Infrastructure.Services;

public class BlogService : IBlogService
{
    private readonly MeetSpotAppDbContext _context;
    private readonly IMapper _mapper;

    public BlogService(MeetSpotAppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<BlogPostResponseDto>> GetPublishedAsync()
    {
        var posts = await _context.BlogPosts
            .Include(p => p.Author)
            .Where(p => p.IsPublished)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return posts.Select(p => _mapper.Map<BlogPostResponseDto>(p)).ToList();
    }

    public async Task<List<BlogPostResponseDto>> GetAllAsync()
    {
        var posts = await _context.BlogPosts
            .Include(p => p.Author)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return posts.Select(p => _mapper.Map<BlogPostResponseDto>(p)).ToList();
    }

    public async Task<BlogPostResponseDto> GetByIdAsync(int id)
    {
        var post = await _context.BlogPosts
            .Include(p => p.Author)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (post == null)
            throw new NotFoundException("Blog post not found");

        return _mapper.Map<BlogPostResponseDto>(post);
    }

    public async Task<BlogPostResponseDto> CreateAsync(CreateBlogPostDto dto, int authorId)
    {
        var post = _mapper.Map<BlogPost>(dto);
        post.AuthorId = authorId;

        _context.BlogPosts.Add(post);
        await _context.SaveChangesAsync();

        var created = await _context.BlogPosts
            .Include(p => p.Author)
            .FirstOrDefaultAsync(p => p.Id == post.Id);

        return _mapper.Map<BlogPostResponseDto>(created!);
    }

    public async Task<BlogPostResponseDto> UpdateAsync(int id, CreateBlogPostDto dto)
    {
        var post = await _context.BlogPosts
            .Include(p => p.Author)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (post == null)
            throw new NotFoundException("Blog post not found");

        post.Title = dto.Title;
        post.Content = dto.Content;
        post.ImageUrl = dto.ImageUrl;
        post.Category = dto.Category;
        post.IsPublished = dto.IsPublished;
        post.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return _mapper.Map<BlogPostResponseDto>(post);
    }

    public async Task DeleteAsync(int id)
    {
        var post = await _context.BlogPosts.FindAsync(id);

        if (post == null)
            throw new NotFoundException("Blog post not found");

        _context.BlogPosts.Remove(post);
        await _context.SaveChangesAsync();
    }
}