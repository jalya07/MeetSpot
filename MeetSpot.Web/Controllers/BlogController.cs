using Microsoft.AspNetCore.Mvc;
using MeetSpot.Web.Services;

namespace MeetSpot.Web.Controllers;

public class BlogController : BaseController
{
    private readonly BlogApiService _blogApiService;

    public BlogController(BlogApiService blogApiService)
    {
        _blogApiService = blogApiService;
    }

    public async Task<IActionResult> Index()
    {
        var posts = await _blogApiService.GetPublishedAsync();
        return View(posts);
    }

    public async Task<IActionResult> Details(int id)
    {
        var post = await _blogApiService.GetByIdAsync(id);
        if (post == null) return NotFound();
        return View(post);
    }
}