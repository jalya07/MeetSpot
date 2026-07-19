using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MeetSpot.Core.DTOs.Auth;
using MeetSpot.Core.Helpers;
using MeetSpot.Core.Interfaces;

namespace MeetSpot.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    IAuthService authService,
    IValidator<RegisterDto> registerValidator,
    IValidator<LoginDto> loginValidator,
    IValidator<ForgotPasswordDto> forgotPasswordValidator,
    IValidator<ResetPasswordDto> resetPasswordValidator,
    IValidator<VerifyTwoFactorDto> verifyTwoFactorValidator) : ControllerBase
{
    [HttpPost("register")]
    [EnableRateLimiting("auth")]
    [ProducesResponseType(typeof(ResponseModelHelper<AuthResponseDto>), 200)]
    [ProducesResponseType(typeof(ResponseModelHelper<AuthResponseDto>), 400)]
    [ProducesResponseType(typeof(ResponseModelHelper<AuthResponseDto>), 409)]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var validation = await registerValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return BadRequest(ResponseModelHelper<AuthResponseDto>
                .BadRequestResult(validation.Errors
                    .Select(e => e.ErrorMessage).ToArray()));

        var result = await authService.RegisterAsync(dto);
        return Ok(ResponseModelHelper<AuthResponseDto>.SuccessResult(result));
    }

    [HttpPost("login")]
    [EnableRateLimiting("auth")]
    [ProducesResponseType(typeof(ResponseModelHelper<LoginResponseDto>), 200)]
    [ProducesResponseType(typeof(ResponseModelHelper<LoginResponseDto>), 401)]
    public async Task<IActionResult> Login([FromBody] LoginDto dto) 
    {
        var validation = await loginValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return BadRequest(ResponseModelHelper<LoginResponseDto>
            .BadRequestResult(validation.Errors
                .Select(e => e.ErrorMessage).ToArray()));

        var result = await authService.LoginAsync(dto);
        return Ok(ResponseModelHelper<LoginResponseDto>.SuccessResult(result));
    }

    [HttpPost("verify-2fa")]
    [EnableRateLimiting("auth")]
    [ProducesResponseType(typeof(ResponseModelHelper<AuthResponseDto>), 200)]
    [ProducesResponseType(typeof(ResponseModelHelper<AuthResponseDto>), 401)]
    public async Task<IActionResult> VerifyTwoFactor([FromBody] VerifyTwoFactorDto dto)
    {
        var validation = await verifyTwoFactorValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return BadRequest(ResponseModelHelper<AuthResponseDto>
                .BadRequestResult(validation.Errors
                    .Select(e => e.ErrorMessage).ToArray()));

        var result = await authService.VerifyTwoFactorAsync(dto);
        return Ok(ResponseModelHelper<AuthResponseDto>.SuccessResult(result));
    }

    [HttpPost("refresh")]
    [ProducesResponseType(typeof(ResponseModelHelper<AuthResponseDto>), 200)]
    [ProducesResponseType(typeof(ResponseModelHelper<AuthResponseDto>), 401)]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto dto)
    {
        var result = await authService.RefreshTokenAsync(dto.AccessToken, dto.RefreshToken);
        return Ok(ResponseModelHelper<AuthResponseDto>.SuccessResult(result));
    }

    [HttpGet("confirm-email")]
    [ProducesResponseType(typeof(ResponseModelHelper<string>), 200)]
    [ProducesResponseType(typeof(ResponseModelHelper<string>), 400)]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string email, [FromQuery] string token)
    {
        await authService.ConfirmEmailAsync(email, token);
        return Ok(ResponseModelHelper<string>.SuccessResult("Email confirmed successfully"));
    }

    [HttpPost("forgot-password")]
    [EnableRateLimiting("auth")]
    [ProducesResponseType(typeof(ResponseModelHelper<string>), 200)]
    [ProducesResponseType(typeof(ResponseModelHelper<string>), 404)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
    {
         var validation = await forgotPasswordValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return BadRequest(ResponseModelHelper<string>
                .BadRequestResult(validation.Errors
                    .Select(e => e.ErrorMessage).ToArray()));
        await authService.ForgotPasswordAsync(dto.Email);
        return Ok(ResponseModelHelper<string>.SuccessResult("Password reset email sent"));
    }

    [HttpPost("reset-password")]
    [EnableRateLimiting("auth")]
    [ProducesResponseType(typeof(ResponseModelHelper<string>), 200)]
    [ProducesResponseType(typeof(ResponseModelHelper<string>), 400)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
         var validation = await resetPasswordValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return BadRequest(ResponseModelHelper<string>
                .BadRequestResult(validation.Errors
                    .Select(e => e.ErrorMessage).ToArray()));
        await authService.ResetPasswordAsync(dto.Token, dto.NewPassword);
        return Ok(ResponseModelHelper<string>.SuccessResult("Password reset successfully"));
    }
}
