using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeetSpot.Core.DTOs.Blog;
using MeetSpot.Core.Helpers;
using MeetSpot.Core.Interfaces;
using System.Security.Claims;

namespace MeetSpot.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BlogController(
    IBlogService blogService,
    IValidator<CreateBlogPostDto> createBlogPostValidator) : ControllerBase
{
    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    [ProducesResponseType(typeof(ResponseModelHelper<List<BlogPostResponseDto>>), 200)]
    public async Task<IActionResult> GetPublished()
    {
        var result = await blogService.GetPublishedAsync();
        return Ok(ResponseModelHelper<List<BlogPostResponseDto>>.SuccessResult(result));
    }

    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ResponseModelHelper<List<BlogPostResponseDto>>), 200)]
    public async Task<IActionResult> GetAll()
    {
        var result = await blogService.GetAllAsync();
        return Ok(ResponseModelHelper<List<BlogPostResponseDto>>.SuccessResult(result));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ResponseModelHelper<BlogPostResponseDto>), 200)]
    [ProducesResponseType(typeof(ResponseModelHelper<BlogPostResponseDto>), 404)]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await blogService.GetByIdAsync(id);
        return Ok(ResponseModelHelper<BlogPostResponseDto>.SuccessResult(result));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ResponseModelHelper<BlogPostResponseDto>), 201)]
    [ProducesResponseType(typeof(ResponseModelHelper<BlogPostResponseDto>), 400)]
    public async Task<IActionResult> Create([FromBody] CreateBlogPostDto dto)
    {
        var validation = await createBlogPostValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return BadRequest(ResponseModelHelper<BlogPostResponseDto>
                .BadRequestResult(validation.Errors
                    .Select(e => e.ErrorMessage).ToArray()));

        var result = await blogService.CreateAsync(dto, GetUserId());
        return StatusCode(201, ResponseModelHelper<BlogPostResponseDto>.CreatedResult(result));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ResponseModelHelper<BlogPostResponseDto>), 200)]
    [ProducesResponseType(typeof(ResponseModelHelper<BlogPostResponseDto>), 404)]
    public async Task<IActionResult> Update(int id, [FromBody] CreateBlogPostDto dto)
    {
        var result = await blogService.UpdateAsync(id, dto);
        return Ok(ResponseModelHelper<BlogPostResponseDto>.SuccessResult(result));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ResponseModelHelper<string>), 200)]
    [ProducesResponseType(typeof(ResponseModelHelper<string>), 404)]
    public async Task<IActionResult> Delete(int id)
    {
        await blogService.DeleteAsync(id);
        return Ok(ResponseModelHelper<string>.SuccessResult("Blog post deleted successfully"));
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

        var fileName = $"blog_{Guid.NewGuid()}{extension}";
        var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "blog");
        Directory.CreateDirectory(uploadsPath);

        var filePath = Path.Combine(uploadsPath, fileName);
        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        var imageUrl = $"{Request.Scheme}://{Request.Host}/images/blog/{fileName}";
        return Ok(ResponseModelHelper<string>.SuccessResult(imageUrl));
    }
}