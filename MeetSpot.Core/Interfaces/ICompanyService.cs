using MeetSpot.Core.DTOs.Company;

namespace MeetSpot.Core.Interfaces;

public interface ICompanyService
{
    Task<List<CompanyResponseDto>> GetAllAsync();
    Task<CompanyResponseDto> GetByIdAsync(int id);
    Task<CompanyResponseDto> CreateAsync(CreateCompanyDto dto);
    Task<CompanyResponseDto> UpdateAsync(int id, CreateCompanyDto dto);
    Task DeleteAsync(int id);
    Task<bool> IsDomainRegisteredAsync(string domain);
}