using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeetSpot.Core.DTOs.User;
using MeetSpot.Core.Helpers;
using MeetSpot.Core.Interfaces;
using System.Security.Claims;

namespace MeetSpot.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController(IUserService userService) : ControllerBase
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
}