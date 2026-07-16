using MeetSpot.Core.DTOs.Company;
using MeetSpot.Core.Entities;
using MeetSpot.Core.Interfaces;
using MeetSpot.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MeetSpot.Infrastructure.Services;

public class CompanyRequestService : ICompanyRequestService
{
    private readonly MeetSpotAppDbContext _context;

    public CompanyRequestService(MeetSpotAppDbContext context)
    {
        _context = context;
    }

    public async Task<CompanyRequestResponseDto> CreateRequestAsync(CreateCompanyRequestDto dto)
    {
        var request = new CompanyRequest
        {
            CompanyName = dto.CompanyName,
            Domain = dto.Domain,
            ContactEmail = dto.ContactEmail,
            Industry = dto.Industry,
            LogoUrl = dto.LogoUrl,
            Message = dto.Message,
            Status = CompanyRequestStatus.Pending
        };

        _context.CompanyRequests.Add(request);
        await _context.SaveChangesAsync();

        return MapToDto(request);
    }

    public async Task<List<CompanyRequestResponseDto>> GetAllAsync()
    {
        var requests = await _context.CompanyRequests
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return requests.Select(MapToDto).ToList();
    }

    public async Task ApproveAsync(int id)
    {
        var request = await _context.CompanyRequests.FindAsync(id);
        if (request == null) throw new Exception("Request not found");

        request.Status = CompanyRequestStatus.Approved;
        request.UpdatedAt = DateTime.UtcNow;

        // Создаём компанию автоматически
        var existingCompany = await _context.Companies
            .FirstOrDefaultAsync(c => c.Domain == request.Domain);

        if (existingCompany == null)
        {
            _context.Companies.Add(new Company
            {
                Name = request.CompanyName,
                Domain = request.Domain,
                Industry = request.Industry,
                LogoUrl = request.LogoUrl,
                IsActive = true
            });
        }

        await _context.SaveChangesAsync();
    }

    public async Task RejectAsync(int id)
    {
        var request = await _context.CompanyRequests.FindAsync(id);
        if (request == null) throw new Exception("Request not found");

        request.Status = CompanyRequestStatus.Rejected;
        request.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    private CompanyRequestResponseDto MapToDto(CompanyRequest r) => new()
    {
        Id = r.Id,
        CompanyName = r.CompanyName,
        Domain = r.Domain,
        ContactEmail = r.ContactEmail,
        Industry = r.Industry,
        LogoUrl = r.LogoUrl,
        Message = r.Message,
        Status = r.Status.ToString(),
        CreatedAt = r.CreatedAt
    };
}