using MeetSpot.Core.DTOs.Company;
using MeetSpot.Core.Helpers;
using MeetSpot.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeetSpot.API.Controllers;

[ApiController]
[Route("api/company-request")]
public class CompanyRequestController : ControllerBase
{
    private readonly ICompanyRequestService _companyRequestService;

    public CompanyRequestController(ICompanyRequestService companyRequestService)
    {
        _companyRequestService = companyRequestService;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Create([FromBody] CreateCompanyRequestDto dto)
    {
        var result = await _companyRequestService.CreateRequestAsync(dto);
        return Ok(ResponseModelHelper<CompanyRequestResponseDto>.SuccessResult(result));
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _companyRequestService.GetAllAsync();
        return Ok(ResponseModelHelper<List<CompanyRequestResponseDto>>.SuccessResult(result));
    }

    [HttpPost("{id}/approve")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Approve(int id)
    {
        await _companyRequestService.ApproveAsync(id);
        return Ok(ResponseModelHelper<string>.SuccessResult("Company request approved"));
    }

    [HttpPost("{id}/reject")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Reject(int id)
    {
        await _companyRequestService.RejectAsync(id);
        return Ok(ResponseModelHelper<string>.SuccessResult("Company request rejected"));
    }
}