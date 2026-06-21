using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeetSpot.Core.DTOs.Company;
using MeetSpot.Core.Helpers;
using MeetSpot.Core.Interfaces;

namespace MeetSpot.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompanyController(
    ICompanyService companyService,
    IValidator<CreateCompanyDto> createCompanyValidator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ResponseModelHelper<List<CompanyResponseDto>>), 200)]
    public async Task<IActionResult> GetAll()
    {
        var result = await companyService.GetAllAsync();
        return Ok(ResponseModelHelper<List<CompanyResponseDto>>.SuccessResult(result));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ResponseModelHelper<CompanyResponseDto>), 200)]
    [ProducesResponseType(typeof(ResponseModelHelper<CompanyResponseDto>), 404)]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await companyService.GetByIdAsync(id);
        return Ok(ResponseModelHelper<CompanyResponseDto>.SuccessResult(result));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ResponseModelHelper<CompanyResponseDto>), 201)]
    [ProducesResponseType(typeof(ResponseModelHelper<CompanyResponseDto>), 400)]
    [ProducesResponseType(typeof(ResponseModelHelper<CompanyResponseDto>), 409)]
    public async Task<IActionResult> Create([FromBody] CreateCompanyDto dto)
    {
        var validation = await createCompanyValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return BadRequest(ResponseModelHelper<CompanyResponseDto>
                .BadRequestResult(validation.Errors
                    .Select(e => e.ErrorMessage).ToArray()));

        var result = await companyService.CreateAsync(dto);
        return StatusCode(201, ResponseModelHelper<CompanyResponseDto>.CreatedResult(result));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ResponseModelHelper<CompanyResponseDto>), 200)]
    [ProducesResponseType(typeof(ResponseModelHelper<CompanyResponseDto>), 404)]
    public async Task<IActionResult> Update(int id, [FromBody] CreateCompanyDto dto)
    {
        var result = await companyService.UpdateAsync(id, dto);
        return Ok(ResponseModelHelper<CompanyResponseDto>.SuccessResult(result));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ResponseModelHelper<string>), 200)]
    [ProducesResponseType(typeof(ResponseModelHelper<string>), 404)]
    public async Task<IActionResult> Delete(int id)
    {
        await companyService.DeleteAsync(id);
        return Ok(ResponseModelHelper<string>.SuccessResult("Company deleted successfully"));
    }
}