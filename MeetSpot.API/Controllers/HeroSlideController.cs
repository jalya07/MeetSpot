using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeetSpot.Core.DTOs.HeroSlide;
using MeetSpot.Core.Helpers;
using MeetSpot.Core.Interfaces;

namespace MeetSpot.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HeroSlideController(IHeroSlideService heroSlideService) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ResponseModelHelper<List<HeroSlideResponseDto>>), 200)]
    public async Task<IActionResult> GetActive()
    {
        var result = await heroSlideService.GetActiveAsync();
        return Ok(ResponseModelHelper<List<HeroSlideResponseDto>>.SuccessResult(result));
    }

    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ResponseModelHelper<List<HeroSlideResponseDto>>), 200)]
    public async Task<IActionResult> GetAll()
    {
        var result = await heroSlideService.GetAllAsync();
        return Ok(ResponseModelHelper<List<HeroSlideResponseDto>>.SuccessResult(result));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ResponseModelHelper<HeroSlideResponseDto>), 201)]
    public async Task<IActionResult> Create([FromBody] CreateHeroSlideDto dto)
    {
        var result = await heroSlideService.CreateAsync(dto);
        return Created(string.Empty, ResponseModelHelper<HeroSlideResponseDto>.CreatedResult(result));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ResponseModelHelper<HeroSlideResponseDto>), 200)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateHeroSlideDto dto)
    {
        var result = await heroSlideService.UpdateAsync(id, dto);
        return Ok(ResponseModelHelper<HeroSlideResponseDto>.SuccessResult(result));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Delete(int id)
    {
        await heroSlideService.DeleteAsync(id);
        return NoContent();
    }
}