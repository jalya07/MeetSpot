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
}