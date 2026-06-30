using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeetSpot.Core.DTOs.Review;
using MeetSpot.Core.Helpers;
using MeetSpot.Core.Interfaces;
using System.Security.Claims;

namespace MeetSpot.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewController(IReviewService reviewService, IValidator<CreateReviewDto> createReviewValidator) : ControllerBase
{
    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ResponseModelHelper<ReviewResponseDto>), 201)]
    public async Task<IActionResult> Create([FromBody] CreateReviewDto dto)
    {
        var validation = await createReviewValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return BadRequest(ResponseModelHelper<ReviewResponseDto>
                .BadRequestResult(validation.Errors.Select(e => e.ErrorMessage).ToArray()));

        var result = await reviewService.CreateAsync(dto, GetUserId());
        return Created(string.Empty, ResponseModelHelper<ReviewResponseDto>.CreatedResult(result));
    }

    [HttpGet("featured")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ResponseModelHelper<List<ReviewResponseDto>>), 200)]
    public async Task<IActionResult> GetFeatured()
    {
        var result = await reviewService.GetFeaturedAsync();
        return Ok(ResponseModelHelper<List<ReviewResponseDto>>.SuccessResult(result));
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ResponseModelHelper<List<ReviewResponseDto>>), 200)]
    public async Task<IActionResult> GetAll()
    {
        var result = await reviewService.GetAllAsync();
        return Ok(ResponseModelHelper<List<ReviewResponseDto>>.SuccessResult(result));
    }

    [HttpPost("{id}/approve")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Approve(int id)
    {
        await reviewService.ApproveAsync(id);
        return NoContent();
    }

    [HttpPost("{id}/featured")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> SetFeatured(int id, [FromQuery] bool isFeatured)
    {
        await reviewService.SetFeaturedAsync(id, isFeatured);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Delete(int id)
    {
        await reviewService.DeleteAsync(id);
        return NoContent();
    }
}