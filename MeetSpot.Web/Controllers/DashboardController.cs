using Microsoft.AspNetCore.Mvc;
using MeetSpot.Web.Services;

namespace MeetSpot.Web.Controllers;

public class DashboardController : BaseController
{
    private readonly BookingApiService _bookingApiService;
    private readonly UserApiService _userApiService;

    public DashboardController(BookingApiService bookingApiService, UserApiService userApiService)
    {
        _bookingApiService = bookingApiService;
        _userApiService = userApiService;
    }

    public async Task<IActionResult> Index()
    {
        var token = Request.Cookies["AccessToken"];
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Account");

        var bookings = await _bookingApiService.GetMyBookingsAsync(token);
        var profile = await _userApiService.GetProfileAsync(token);

        ViewBag.Bookings = bookings;
        ViewBag.Profile = profile;

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        var token = Request.Cookies["AccessToken"];
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Account");

        var profile = await _userApiService.GetProfileAsync(token);
        ViewBag.Profile = profile;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Profile(string firstName, string lastName)
    {
        var token = Request.Cookies["AccessToken"];
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Account");

        await _userApiService.UpdateProfileAsync(token, firstName, lastName);
        TempData["Success"] = "Profile updated! Log out and back in to see name changes.";
        return RedirectToAction("Profile");
    }

    [HttpPost]
    public async Task<IActionResult> UploadAvatar(IFormFile avatarFile)
    {
        var token = Request.Cookies["AccessToken"];
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Account");

        if (avatarFile == null || avatarFile.Length == 0)
        {
            TempData["Error"] = "Please select a file.";
            return RedirectToAction("Profile");
        }

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var extension = Path.GetExtension(avatarFile.FileName).ToLower();
        if (!allowedExtensions.Contains(extension))
        {
            TempData["Error"] = "Only JPG, PNG or WEBP files are allowed.";
            return RedirectToAction("Profile");
        }

        var result = await _userApiService.UploadAvatarAsync(token, avatarFile);
        if (result != null)
            TempData["Success"] = "Avatar updated! Log out and back in to see it everywhere.";
        else
            TempData["Error"] = "Failed to upload avatar. Please try again.";

        return RedirectToAction("Profile");
    }
}