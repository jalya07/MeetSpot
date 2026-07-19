using System.Text.Json;
using MeetSpot.Core.DTOs.Mentor;
using MeetSpot.Core.Entities;
using MeetSpot.Core.Interfaces;
using MeetSpot.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MeetSpot.Infrastructure.Services;

public class MentorService : IMentorService
{
    private readonly MeetSpotAppDbContext _context;

    public MentorService(MeetSpotAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<MentorResponseDto>> GetAllAsync()
    {
        var mentors = await _context.Mentors
            .Where(m => m.IsActive)
            .OrderBy(m => m.FirstName)
            .ToListAsync();

        return mentors.Select(MapToDto).ToList();
    }

    public async Task<MentorResponseDto> CreateAsync(CreateMentorDto dto)
    {
        var mentor = new Mentor
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Role = dto.Role,
            Company = dto.Company,
            Bio = dto.Bio,
            Skills = JsonSerializer.Serialize(dto.Skills),
            Rating = dto.Rating,
            SessionsCount = dto.SessionsCount,
            IsActive = true
        };

        _context.Mentors.Add(mentor);
        await _context.SaveChangesAsync();
        return MapToDto(mentor);
    }

    public async Task DeleteAsync(int id)
    {
        var mentor = await _context.Mentors.FindAsync(id);
        if (mentor != null)
        {
            mentor.IsActive = false;
            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdateAvatarAsync(int id, string avatarUrl)
    {
        var mentor = await _context.Mentors.FindAsync(id);
        if (mentor != null)
        {
            mentor.AvatarUrl = avatarUrl;
            await _context.SaveChangesAsync();
        }
    }

    private MentorResponseDto MapToDto(Mentor m) => new()
    {
        Id = m.Id,
        FirstName = m.FirstName,
        LastName = m.LastName,
        Role = m.Role,
        Company = m.Company,
        Bio = m.Bio,
        AvatarUrl = m.AvatarUrl,
        Skills = string.IsNullOrEmpty(m.Skills)
            ? new List<string>()
            : JsonSerializer.Deserialize<List<string>>(m.Skills) ?? new List<string>(),
        Rating = m.Rating,
        SessionsCount = m.SessionsCount,
        IsActive = m.IsActive
    };
    
    public async Task<MentorRequestResponseDto> CreateRequestAsync(CreateMentorRequestDto dto)
    {
        var request = new MentorRequest
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Role = dto.Role,
            Company = dto.Company,
            Bio = dto.Bio,
            Skills = dto.Skills,
            LinkedInUrl = dto.LinkedInUrl,
            Status = MentorRequestStatus.Pending
        };

        _context.MentorRequests.Add(request);
        await _context.SaveChangesAsync();
        return MapRequestToDto(request);
    }

    public async Task<List<MentorRequestResponseDto>> GetAllRequestsAsync()
    {
        var requests = await _context.MentorRequests
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
        return requests.Select(MapRequestToDto).ToList();
    }

    public async Task ApproveRequestAsync(int id)
    {
        var request = await _context.MentorRequests.FindAsync(id);
        if (request == null) return;

        request.Status = MentorRequestStatus.Approved;
        request.UpdatedAt = DateTime.UtcNow;

    
        var skills = request.Skills?.Split(',').Select(s => s.Trim()).ToList() ?? new();
        _context.Mentors.Add(new Mentor
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Role = request.Role,
            Company = request.Company,
            Bio = request.Bio,
            Skills = JsonSerializer.Serialize(skills),
            Rating = 5,
            SessionsCount = 0,
            IsActive = true
        });

        await _context.SaveChangesAsync();
    }

    public async Task RejectRequestAsync(int id)
    {
        var request = await _context.MentorRequests.FindAsync(id);
        if (request == null) return;

        request.Status = MentorRequestStatus.Rejected;
        request.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    private MentorRequestResponseDto MapRequestToDto(MentorRequest r) => new()
    {
        Id = r.Id,
        FirstName = r.FirstName,
        LastName = r.LastName,
        Email = r.Email,
        Role = r.Role,
        Company = r.Company,
        Bio = r.Bio,
        Skills = r.Skills,
        LinkedInUrl = r.LinkedInUrl,
        Status = r.Status.ToString(),
        CreatedAt = r.CreatedAt
    };
}