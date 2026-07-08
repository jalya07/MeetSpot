using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeetSpot.Core.DTOs.Room;
using MeetSpot.Core.Helpers;
using MeetSpot.Core.Interfaces;

namespace MeetSpot.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomController(
    IRoomService roomService,
    IValidator<CreateRoomDto> createRoomValidator,
    IValidator<UpdateRoomDto> updateRoomValidator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ResponseModelHelper<List<RoomResponseDto>>), 200)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? type,
        [FromQuery] int? capacity,
        [FromQuery] int? floor)
    {
        var rooms = await roomService.GetAllAsync(type, capacity, floor);
        return Ok(ResponseModelHelper<List<RoomResponseDto>>.SuccessResult(rooms));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ResponseModelHelper<RoomResponseDto>), 200)]
    [ProducesResponseType(typeof(ResponseModelHelper<RoomResponseDto>), 404)]
    public async Task<IActionResult> GetById(int id)
    {
        var room = await roomService.GetByIdAsync(id);
        return Ok(ResponseModelHelper<RoomResponseDto>.SuccessResult(room));
    }

    [HttpGet("available")]
    [ProducesResponseType(typeof(ResponseModelHelper<List<RoomResponseDto>>), 200)]
    public async Task<IActionResult> GetAvailable(
        [FromQuery] DateTime startTime,
        [FromQuery] DateTime endTime,
        [FromQuery] int? capacity)
    {
        var rooms = await roomService.GetAvailableRoomsAsync(startTime, endTime, capacity);
        return Ok(ResponseModelHelper<List<RoomResponseDto>>.SuccessResult(rooms));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ResponseModelHelper<RoomResponseDto>), 201)]
    [ProducesResponseType(typeof(ResponseModelHelper<RoomResponseDto>), 400)]
    public async Task<IActionResult> Create([FromBody] CreateRoomDto dto)
    {
        var validation = await createRoomValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return BadRequest(ResponseModelHelper<RoomResponseDto>
                .BadRequestResult(validation.Errors
                    .Select(e => e.ErrorMessage).ToArray()));

        var room = await roomService.CreateAsync(dto);
        return StatusCode(201, ResponseModelHelper<RoomResponseDto>.CreatedResult(room));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ResponseModelHelper<RoomResponseDto>), 200)]
    [ProducesResponseType(typeof(ResponseModelHelper<RoomResponseDto>), 404)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateRoomDto dto)
    {
        var validation = await updateRoomValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return BadRequest(ResponseModelHelper<RoomResponseDto>
                .BadRequestResult(validation.Errors
                    .Select(e => e.ErrorMessage).ToArray()));
        var room = await roomService.UpdateAsync(id, dto);
        return Ok(ResponseModelHelper<RoomResponseDto>.SuccessResult(room));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ResponseModelHelper<string>), 200)]
    [ProducesResponseType(typeof(ResponseModelHelper<string>), 404)]
    public async Task<IActionResult> Delete(int id)
    {
        await roomService.DeleteAsync(id);
        return Ok(ResponseModelHelper<string>.SuccessResult("Room deleted successfully"));
    }

    [HttpPost("upload-image")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(ResponseModelHelper<string>.BadRequestResult("No file provided"));

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var extension = Path.GetExtension(file.FileName).ToLower();

        if (!allowedExtensions.Contains(extension))
            return BadRequest(ResponseModelHelper<string>.BadRequestResult("Only jpg, png, webp allowed"));

        var fileName = $"room_{Guid.NewGuid()}{extension}";
        var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "rooms");
        Directory.CreateDirectory(uploadsPath);

        var filePath = Path.Combine(uploadsPath, fileName);
        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        var imageUrl = $"{Request.Scheme}://{Request.Host}/images/rooms/{fileName}";
        return Ok(ResponseModelHelper<string>.SuccessResult(imageUrl));
    }
}