using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MeetSpot.Core.DTOs.Balance;
using MeetSpot.Core.Helpers;
using MeetSpot.Core.Interfaces;
using System.Security.Claims;

namespace MeetSpot.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BalanceController(
    IBalanceService balanceService,
    IValidator<TopUpBalanceDto> topUpValidator) : ControllerBase
{
    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost("topup")]
    [EnableRateLimiting("auth")]
    [ProducesResponseType(typeof(ResponseModelHelper<TransactionResponseDto>), 200)]
    [ProducesResponseType(typeof(ResponseModelHelper<TransactionResponseDto>), 400)]
    public async Task<IActionResult> TopUp([FromBody] TopUpBalanceDto dto)
    {
        var validation = await topUpValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return BadRequest(ResponseModelHelper<TransactionResponseDto>
                .BadRequestResult(validation.Errors
                    .Select(e => e.ErrorMessage).ToArray()));

        var result = await balanceService.TopUpAsync(GetUserId(), dto);
        return Ok(ResponseModelHelper<TransactionResponseDto>.SuccessResult(result));
    }

    [HttpGet("transactions")]
    [ProducesResponseType(typeof(ResponseModelHelper<List<TransactionResponseDto>>), 200)]
    public async Task<IActionResult> GetTransactions()
    {
        var result = await balanceService.GetTransactionHistoryAsync(GetUserId());
        return Ok(ResponseModelHelper<List<TransactionResponseDto>>.SuccessResult(result));
    }

    [HttpGet]
    [ProducesResponseType(typeof(ResponseModelHelper<decimal>), 200)]
    public async Task<IActionResult> GetBalance()
    {
        var result = await balanceService.GetBalanceAsync(GetUserId());
        return Ok(ResponseModelHelper<decimal>.SuccessResult(result));
    }
}