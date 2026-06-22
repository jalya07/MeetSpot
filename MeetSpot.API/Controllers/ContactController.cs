using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeetSpot.Core.DTOs.Contact;
using MeetSpot.Core.Helpers;
using MeetSpot.Core.Interfaces;

namespace MeetSpot.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactController(
    IContactService contactService,
    IValidator<CreateContactMessageDto> createContactValidator) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ResponseModelHelper<ContactMessageResponseDto>), 201)]
    [ProducesResponseType(typeof(ResponseModelHelper<ContactMessageResponseDto>), 400)]
    public async Task<IActionResult> Create([FromBody] CreateContactMessageDto dto)
    {
        var validation = await createContactValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return BadRequest(ResponseModelHelper<ContactMessageResponseDto>
                .BadRequestResult(validation.Errors
                    .Select(e => e.ErrorMessage).ToArray()));

        var result = await contactService.CreateAsync(dto);
        return StatusCode(201, ResponseModelHelper<ContactMessageResponseDto>.CreatedResult(result));
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ResponseModelHelper<List<ContactMessageResponseDto>>), 200)]
    public async Task<IActionResult> GetAll()
    {
        var result = await contactService.GetAllAsync();
        return Ok(ResponseModelHelper<List<ContactMessageResponseDto>>.SuccessResult(result));
    }

    [HttpPost("{id}/read")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ResponseModelHelper<string>), 200)]
    [ProducesResponseType(typeof(ResponseModelHelper<string>), 404)]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        await contactService.MarkAsReadAsync(id);
        return Ok(ResponseModelHelper<string>.SuccessResult("Message marked as read"));
    }
}