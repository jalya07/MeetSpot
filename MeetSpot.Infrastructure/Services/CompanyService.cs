using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MeetSpot.Core.DTOs.Company;
using MeetSpot.Core.Entities;
using MeetSpot.Core.Exceptions;
using MeetSpot.Core.Interfaces;
using MeetSpot.Infrastructure.Data;

namespace MeetSpot.Infrastructure.Services;

public class CompanyService : ICompanyService
{
    private readonly MeetSpotAppDbContext _context;
    private readonly IMapper _mapper;

    public CompanyService(MeetSpotAppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<CompanyResponseDto>> GetAllAsync()
    {
        var companies = await _context.Companies
            .Where(c => c.IsActive)
            .ToListAsync();

        return companies.Select(c => _mapper.Map<CompanyResponseDto>(c)).ToList();
    }

    public async Task<CompanyResponseDto> GetByIdAsync(int id)
    {
        var company = await _context.Companies.FindAsync(id);

        if (company == null)
            throw new NotFoundException("Company not found");

        return _mapper.Map<CompanyResponseDto>(company);
    }

    public async Task<CompanyResponseDto> CreateAsync(CreateCompanyDto dto)
    {
        var existing = await _context.Companies
            .FirstOrDefaultAsync(c => c.Domain == dto.Domain);

        if (existing != null)
            throw new ConflictException("A company with this domain already exists");

        var company = _mapper.Map<Company>(dto);
        company.IsActive = true;

        _context.Companies.Add(company);
        await _context.SaveChangesAsync();

        return _mapper.Map<CompanyResponseDto>(company);
    }

    public async Task<CompanyResponseDto> UpdateAsync(int id, CreateCompanyDto dto)
    {
        var company = await _context.Companies.FindAsync(id);

        if (company == null)
            throw new NotFoundException("Company not found");

        company.Name = dto.Name;
        company.Domain = dto.Domain;
        company.LogoUrl = dto.LogoUrl;
        company.Industry = dto.Industry;
        company.ContractStartDate = dto.ContractStartDate;
        company.ContractEndDate = dto.ContractEndDate;
        company.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return _mapper.Map<CompanyResponseDto>(company);
    }

    public async Task DeleteAsync(int id)
    {
        var company = await _context.Companies.FindAsync(id);

        if (company == null)
            throw new NotFoundException("Company not found");

        company.IsActive = false;
        company.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsDomainRegisteredAsync(string domain)
    {
        return await _context.Companies
            .AnyAsync(c => c.Domain == domain && c.IsActive);
    }
}