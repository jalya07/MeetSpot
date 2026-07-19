using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeetSpot.Core.DTOs.User;
using MeetSpot.Core.Helpers;
using MeetSpot.Core.Interfaces;
using System.Security.Claims;
using FluentValidation;

namespace MeetSpot.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController(IUserService userService,IValidator<UpdateProfileDto> updateProfileValidator) : ControllerBase
{
    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("me")]
    [ProducesResponseType(typeof(ResponseModelHelper<UserProfileDto>), 200)]
    public async Task<IActionResult> GetMe()
    {
        var result = await userService.GetProfileAsync(GetUserId());
        return Ok(ResponseModelHelper<UserProfileDto>.SuccessResult(result));
    }

    [HttpPut("me")]
    [ProducesResponseType(typeof(ResponseModelHelper<UserProfileDto>), 200)]
    public async Task<IActionResult> UpdateMe([FromBody] UpdateProfileDto dto)
    {
        var validation = await updateProfileValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return BadRequest(ResponseModelHelper<UserProfileDto>
                .BadRequestResult(validation.Errors
                    .Select(e => e.ErrorMessage).ToArray()));

        var result = await userService.UpdateProfileAsync(GetUserId(), dto);
        return Ok(ResponseModelHelper<UserProfileDto>.SuccessResult(result));
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ResponseModelHelper<List<UserListItemDto>>), 200)]
    public async Task<IActionResult> GetAllUsers()
    {
        var result = await userService.GetAllUsersAsync();
    return Ok(ResponseModelHelper<List<UserListItemDto>>.SuccessResult(result));
    }

    [HttpPost("{id}/toggle-status")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ResponseModelHelper<string>), 200)]
    [ProducesResponseType(typeof(ResponseModelHelper<string>), 404)]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        await userService.ToggleUserStatusAsync(id);
        return Ok(ResponseModelHelper<string>.SuccessResult("User status updated successfully"));
    }

    [HttpPost("upload-avatar")]
    [Authorize]
    public async Task<IActionResult> UploadAvatar(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(ResponseModelHelper<string>.BadRequestResult("No file provided"));

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var extension = Path.GetExtension(file.FileName).ToLower();

        if (!allowedExtensions.Contains(extension))
            return BadRequest(ResponseModelHelper<string>.BadRequestResult("Only jpg, png, webp allowed"));

        var fileName = $"avatar_{Guid.NewGuid()}{extension}";
        var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "avatars");
        Directory.CreateDirectory(uploadsPath);

        var filePath = Path.Combine(uploadsPath, fileName);
        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        var avatarUrl = $"/images/avatars/{fileName}";

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await userService.UpdateAvatarAsync(userId, avatarUrl);

        return Ok(ResponseModelHelper<string>.SuccessResult(avatarUrl));
    }
}