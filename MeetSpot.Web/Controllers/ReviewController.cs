using Microsoft.AspNetCore.Mvc;
using MeetSpot.Web.Services;

namespace MeetSpot.Web.Controllers;

public class ReviewController : BaseController
{
    private readonly ReviewApiService _reviewApiService;

    public ReviewController(ReviewApiService reviewApiService)
    {
        _reviewApiService = reviewApiService;
    }

    [HttpGet]
    public IActionResult Create()
    {
        var token = Request.Cookies["AccessToken"];
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Account");

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(string text, int rating)
    {
        var token = Request.Cookies["AccessToken"];
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Account");

        await _reviewApiService.CreateReviewAsync(token, text, rating);
        TempData["Success"] = "Your review has been submitted! It will appear after moderation.";
        return RedirectToAction("Index", "Dashboard");
    }
}