using Microsoft.AspNetCore.Mvc;
using MeetSpot.Web.Services;

namespace MeetSpot.Web.Controllers;

public class BookingController : BaseController
{
    private readonly RoomApiService _roomApiService;
    private readonly BookingApiService _bookingApiService;

    public BookingController(RoomApiService roomApiService, BookingApiService bookingApiService)
    {
        _roomApiService = roomApiService;
        _bookingApiService = bookingApiService;
    }

    [HttpGet]
    public async Task<IActionResult> Create(int id)
    {
        var token = Request.Cookies["AccessToken"];
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Account");

        var room = await _roomApiService.GetRoomByIdAsync(id);
        return View(room);
    }

    [HttpPost]
    public async Task<IActionResult> Create(int id, string title, DateTime startTime, DateTime endTime)
    {
        var token = Request.Cookies["AccessToken"];
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Account");

        var (success, errors) = await _bookingApiService.CreateBookingAsync(token, id, title, startTime, endTime);

        if (success)
        {
            TempData["SuccessMessage"] = "Room booked successfully!";
            return RedirectToAction("Index", "Dashboard");
        }

        var room = await _roomApiService.GetRoomByIdAsync(id);
        ViewBag.Errors = errors;
        return View(room);
    }

    [HttpGet]
    public async Task<IActionResult> CancelConfirm(int id)
    {
        var token = Request.Cookies["AccessToken"];
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Account");

        await _bookingApiService.CancelBookingAsync(token, id);
        TempData["Success"] = "Booking cancelled successfully.";
        return RedirectToAction("Index", "Dashboard");
    }

    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Cancel(int id)
    {
        var token = Request.Cookies["AccessToken"];
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Account");

        await _bookingApiService.CancelBookingAsync(token, id);
        TempData["Success"] = "Booking cancelled successfully.";
        return RedirectToAction("Index", "Dashboard");
    }
}