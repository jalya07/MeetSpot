using MeetSpot.Core.DTOs.Company;

namespace MeetSpot.Core.Interfaces;

public interface ICompanyRequestService
{
    Task<CompanyRequestResponseDto> CreateRequestAsync(CreateCompanyRequestDto dto);
    Task<List<CompanyRequestResponseDto>> GetAllAsync();
    Task ApproveAsync(int id);
    Task RejectAsync(int id);
}