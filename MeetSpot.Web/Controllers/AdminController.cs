using Microsoft.AspNetCore.Mvc;
using MeetSpot.Web.Services;

namespace MeetSpot.Web.Controllers;

public class AdminController : BaseController
{
    private readonly AnalyticsApiService _analyticsApiService;
    private readonly RoomApiService _roomApiService;
    private readonly UserApiService _userApiService;
    private readonly BookingApiService _bookingApiService;
    private readonly ReviewApiService _reviewApiService;

    public AdminController(
        AnalyticsApiService analyticsApiService,
        RoomApiService roomApiService,
        UserApiService userApiService,
        BookingApiService bookingApiService,
        ReviewApiService reviewApiService)
    {
        _analyticsApiService = analyticsApiService;
        _roomApiService = roomApiService;
        _userApiService = userApiService;
        _bookingApiService = bookingApiService;
        _reviewApiService = reviewApiService;
    }

    private IActionResult CheckAdmin()
    {
        var token = Request.Cookies["AccessToken"];
        if (string.IsNullOrEmpty(token)) 
            return RedirectToAction("Login", "Account");
        if (ViewBag.UserRole != "Admin")
            return RedirectToAction("Index", "Home");
        return null!;
    }

    public async Task<IActionResult> Dashboard()
    {
        var check = CheckAdmin();
        if (check != null) return check;

        var token = Request.Cookies["AccessToken"]!;
        var stats = await _analyticsApiService.GetPublicStatsAsync();

        ViewBag.Stats = stats;
        return View();
    }

    public async Task<IActionResult> Rooms()
    {
        var check = CheckAdmin();
        if (check != null) 
            return check;

        var rooms = await _roomApiService.GetAllRoomsAsync();
        return View(rooms);
    }

    public async Task<IActionResult> Bookings()
    {
        var check = CheckAdmin();
        if (check != null) 
            return check;

        var token = Request.Cookies["AccessToken"]!;
        var bookings = await _bookingApiService.GetAllBookingsAsync(token);
        return View(bookings);
    }

    public async Task<IActionResult> Users()
    {
        var check = CheckAdmin();
        if (check != null) 
            return check;

        var token = Request.Cookies["AccessToken"]!;
        var users = await _userApiService.GetAllUsersAsync(token);
        return View(users);
    }

    public async Task<IActionResult> Reviews()
    {
        var check = CheckAdmin();
        if (check != null) 
            return check;

        var token = Request.Cookies["AccessToken"]!;
        var reviews = await _reviewApiService.GetAllReviewsAsync(token);
        return View(reviews);
    }
    [HttpPost]
    public async Task<IActionResult> ApproveReview(int id)
    {
        var token = Request.Cookies["AccessToken"]!;
        await _reviewApiService.ApproveReviewAsync(token, id);
        return RedirectToAction("Reviews");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteReview(int id)
    {
        var token = Request.Cookies["AccessToken"]!;
        await _reviewApiService.DeleteReviewAsync(token, id);
        return RedirectToAction("Reviews");
    }

    [HttpPost]
    public async Task<IActionResult> SetFeatured(int id)
    {
        var token = Request.Cookies["AccessToken"]!;
        await _reviewApiService.SetFeaturedAsync(token, id, true);
        return RedirectToAction("Reviews");
    }

    [HttpPost]
    public async Task<IActionResult> UnfeatureReview(int id)
    {
        var token = Request.Cookies["AccessToken"]!;
        await _reviewApiService.SetFeaturedAsync(token, id, false);
        return RedirectToAction("Reviews");
    }
}