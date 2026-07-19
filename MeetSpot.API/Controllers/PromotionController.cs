using MeetSpot.Core.DTOs.Promotion;
using MeetSpot.Core.Helpers;
using MeetSpot.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeetSpot.API.Controllers;

[ApiController]
[Route("api/promotion")]
public class PromotionController : ControllerBase
{
    private readonly IPromotionService _promotionService;

    public PromotionController(IPromotionService promotionService)
    {
        _promotionService = promotionService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetActive()
    {
        var result = await _promotionService.GetActiveAsync();
        return Ok(ResponseModelHelper<List<PromotionResponseDto>>.SuccessResult(result));
    }

    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _promotionService.GetAllAsync();
        return Ok(ResponseModelHelper<List<PromotionResponseDto>>.SuccessResult(result));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreatePromotionDto dto)
    {
        var result = await _promotionService.CreateAsync(dto);
        return Ok(ResponseModelHelper<PromotionResponseDto>.SuccessResult(result));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _promotionService.DeleteAsync(id);
        return Ok(ResponseModelHelper<string>.SuccessResult("Promotion deleted"));
    }
}