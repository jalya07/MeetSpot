using MeetSpot.Core.DTOs.Mentor;
using MeetSpot.Core.Helpers;
using MeetSpot.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeetSpot.API.Controllers;

[ApiController]
[Route("api/mentor")]
public class MentorController : ControllerBase
{
    private readonly IMentorService _mentorService;

    public MentorController(IMentorService mentorService)
    {
        _mentorService = mentorService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mentorService.GetAllAsync();
        return Ok(ResponseModelHelper<List<MentorResponseDto>>.SuccessResult(result));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateMentorDto dto)
    {
        var result = await _mentorService.CreateAsync(dto);
        return Ok(ResponseModelHelper<MentorResponseDto>.SuccessResult(result));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _mentorService.DeleteAsync(id);
        return Ok(ResponseModelHelper<string>.SuccessResult("Mentor deleted"));
    }

    [HttpPost("{id}/upload-avatar")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UploadAvatar(int id, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(ResponseModelHelper<string>.BadRequestResult("No file provided"));

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var extension = Path.GetExtension(file.FileName).ToLower();

        if (!allowedExtensions.Contains(extension))
            return BadRequest(ResponseModelHelper<string>.BadRequestResult("Only jpg, png, webp allowed"));

        var fileName = $"mentor_{Guid.NewGuid()}{extension}";
        var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "mentors");
        Directory.CreateDirectory(uploadsPath);

        var filePath = Path.Combine(uploadsPath, fileName);
        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        var avatarUrl = $"/images/mentors/{fileName}";
        await _mentorService.UpdateAvatarAsync(id, avatarUrl);

        return Ok(ResponseModelHelper<string>.SuccessResult(avatarUrl));
    }

    [HttpPost("request")]
    [AllowAnonymous]
    public async Task<IActionResult> CreateRequest([FromBody] CreateMentorRequestDto dto)
    {
        var result = await _mentorService.CreateRequestAsync(dto);
        return Ok(ResponseModelHelper<MentorRequestResponseDto>.SuccessResult(result));
    }

    [HttpGet("requests")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllRequests()
    {
        var result = await _mentorService.GetAllRequestsAsync();
        return Ok(ResponseModelHelper<List<MentorRequestResponseDto>>.SuccessResult(result));
    }

    [HttpPost("requests/{id}/approve")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ApproveRequest(int id)
    {
        await _mentorService.ApproveRequestAsync(id);
        return Ok(ResponseModelHelper<string>.SuccessResult("Mentor request approved"));
    }

    [HttpPost("requests/{id}/reject")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RejectRequest(int id)
    {
        await _mentorService.RejectRequestAsync(id);
        return Ok(ResponseModelHelper<string>.SuccessResult("Mentor request rejected"));
    }
}