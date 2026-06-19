using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeetSpot.Core.DTOs.Subscription;
using MeetSpot.Core.Helpers;
using MeetSpot.Core.Interfaces;
using System.Security.Claims;

namespace MeetSpot.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SubscriptionController(
    ISubscriptionService subscriptionService,
    IValidator<CreateSubscriptionDto> createSubscriptionValidator) : ControllerBase
{
    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    [ProducesResponseType(typeof(ResponseModelHelper<SubscriptionResponseDto>), 201)]
    [ProducesResponseType(typeof(ResponseModelHelper<SubscriptionResponseDto>), 400)]
    [ProducesResponseType(typeof(ResponseModelHelper<SubscriptionResponseDto>), 409)]
    public async Task<IActionResult> Subscribe([FromBody] CreateSubscriptionDto dto)
    {
        var validation = await createSubscriptionValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return BadRequest(ResponseModelHelper<SubscriptionResponseDto>
                .BadRequestResult(validation.Errors
                    .Select(e => e.ErrorMessage).ToArray()));

        var result = await subscriptionService.SubscribeAsync(GetUserId(), dto);
        return StatusCode(201, ResponseModelHelper<SubscriptionResponseDto>.CreatedResult(result));
    }

    [HttpGet("my")]
    [ProducesResponseType(typeof(ResponseModelHelper<SubscriptionResponseDto>), 200)]
    public async Task<IActionResult> GetMy()
    {
        var result = await subscriptionService.GetMyActiveSubscriptionAsync(GetUserId());
        return Ok(ResponseModelHelper<SubscriptionResponseDto?>.SuccessResult(result));
    }

    [HttpPost("cancel")]
    [ProducesResponseType(typeof(ResponseModelHelper<string>), 200)]
    [ProducesResponseType(typeof(ResponseModelHelper<string>), 404)]
    public async Task<IActionResult> Cancel()
    {
        await subscriptionService.CancelAsync(GetUserId());
        return Ok(ResponseModelHelper<string>.SuccessResult("Subscription cancelled successfully"));
    }
}