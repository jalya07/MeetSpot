using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MeetSpot.Core.DTOs.Booking;
using MeetSpot.Core.Helpers;
using MeetSpot.Core.Interfaces;
using System.Security.Claims;

namespace MeetSpot.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingController(
    IBookingService bookingService,
    IValidator<CreateBookingDto> createBookingValidator,
    IValidator<UpdateBookingDto> updateBookingValidator,
    IBookingNotificationService bookingNotificationService) : ControllerBase
{
    private readonly IBookingNotificationService _bookingNotificationService = bookingNotificationService;
    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    [ProducesResponseType(typeof(ResponseModelHelper<BookingResponseDto>), 201)]
    [ProducesResponseType(typeof(ResponseModelHelper<BookingResponseDto>), 400)]
    [ProducesResponseType(typeof(ResponseModelHelper<BookingResponseDto>), 409)]
    public async Task<IActionResult> Create([FromBody] CreateBookingDto dto)
    {
        var validation = await createBookingValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return BadRequest(ResponseModelHelper<BookingResponseDto>
                .BadRequestResult(validation.Errors
                    .Select(e => e.ErrorMessage).ToArray()));

        var result = await bookingService.CreateAsync(dto, GetUserId());
        return StatusCode(201, ResponseModelHelper<BookingResponseDto>.CreatedResult(result));
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ResponseModelHelper<BookingResponseDto>), 200)]
    [ProducesResponseType(typeof(ResponseModelHelper<BookingResponseDto>), 404)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateBookingDto dto)
    {
        var validation = await updateBookingValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return BadRequest(ResponseModelHelper<BookingResponseDto>
                .BadRequestResult(validation.Errors
                    .Select(e => e.ErrorMessage).ToArray()));
                    
        var result = await bookingService.UpdateAsync(id, dto, GetUserId());
        return Ok(ResponseModelHelper<BookingResponseDto>.SuccessResult(result));
    }

    [HttpPost("{id}/cancel")]
    [ProducesResponseType(typeof(ResponseModelHelper<string>), 200)]
    [ProducesResponseType(typeof(ResponseModelHelper<string>), 400)]
    public async Task<IActionResult> Cancel(int id, [FromBody] string? reason)
    {
        var isAdmin = User.IsInRole("Admin");
        await bookingService.CancelAsync(id, GetUserId(), isAdmin, reason);
        return Ok(ResponseModelHelper<string>.SuccessResult("Booking cancelled successfully"));
    }

    [HttpGet("my")]
    [ProducesResponseType(typeof(ResponseModelHelper<List<BookingResponseDto>>), 200)]
    public async Task<IActionResult> GetMyBookings()
    {
        var result = await bookingService.GetMyBookingsAsync(GetUserId());
        return Ok(ResponseModelHelper<List<BookingResponseDto>>.SuccessResult(result));
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ResponseModelHelper<List<BookingResponseDto>>), 200)]
    public async Task<IActionResult> GetAll()
    {
        var result = await bookingService.GetAllAsync();
        return Ok(ResponseModelHelper<List<BookingResponseDto>>.SuccessResult(result));
    }

    [HttpGet("room/{roomId}/calendar")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ResponseModelHelper<List<BookingResponseDto>>), 200)]
    public async Task<IActionResult> GetRoomCalendar(int roomId, [FromQuery] DateTime date)
    {
        var result = await bookingService.GetRoomCalendarAsync(roomId, date);
        return Ok(ResponseModelHelper<List<BookingResponseDto>>.SuccessResult(result));
    }

    [HttpPost("{id}/checkin")]
    [ProducesResponseType(typeof(ResponseModelHelper<string>), 200)]
    [ProducesResponseType(typeof(ResponseModelHelper<string>), 400)]
    public async Task<IActionResult> CheckIn(int id)
    {
        var isAdmin = User.IsInRole("Admin");
        await bookingService.CheckInAsync(id, GetUserId(), isAdmin);
        return Ok(ResponseModelHelper<string>.SuccessResult("Check-in successful"));
    }

    [HttpPost("test-signalr")]
    [AllowAnonymous]
    public async Task<IActionResult> TestSignalR()
    {
        if (!HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
            return NotFound();

        await _bookingNotificationService.NotifyBookingCreated(1, "Test Room", DateTime.Now, DateTime.Now.AddHours(1));
        return Ok("Sent!");
    }
}