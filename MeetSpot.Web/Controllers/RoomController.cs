using Microsoft.AspNetCore.Mvc;
using MeetSpot.Web.Services;

namespace MeetSpot.Web.Controllers;

public class RoomController : BaseController
{
    private readonly RoomApiService _roomApiService;
    private readonly BookingApiService _bookingApiService;

    public RoomController(RoomApiService roomApiService, BookingApiService bookingApiService)
    {
        _roomApiService = roomApiService;
        _bookingApiService = bookingApiService;
    }

    public async Task<IActionResult> Index(string? type, int? capacity, string? search)
    {
        var rooms = await _roomApiService.GetAllRoomsAsync();

        if (!string.IsNullOrEmpty(type))
            rooms = rooms.Where(r => r.Type.ToLower() == type.ToLower()).ToList();

        if (capacity.HasValue)
            rooms = rooms.Where(r => r.Capacity >= capacity.Value).ToList();

        if (!string.IsNullOrEmpty(search))
            rooms = rooms.Where(r => r.Name.Contains(search, StringComparison.OrdinalIgnoreCase)
                || (r.Description != null && r.Description.Contains(search, StringComparison.OrdinalIgnoreCase))).ToList();

        ViewBag.CurrentType = type;
        ViewBag.CurrentCapacity = capacity;
        ViewBag.CurrentSearch = search;

        return View(rooms);
    }

    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var room = await _roomApiService.GetRoomByIdAsync(id);
            var today = DateTime.UtcNow.Date;
            var calendar = await _bookingApiService.GetRoomCalendarAsync(id, today);
        
            ViewBag.Calendar = calendar;
            ViewBag.SelectedDate = today;

            return View(room);
        }
        catch
        {
            return NotFound();
        }
    }

    public async Task<IActionResult> Calendar(int id, DateTime? date)
    {
        try
        {
            var room = await _roomApiService.GetRoomByIdAsync(id);
            var selectedDate = date?.Date ?? DateTime.UtcNow.Date;
            var token = Request.Cookies["AccessToken"];

            var allBookings = new List<BookingResponseDto>();
            for (int i = 0; i < 7; i++)
            {
                var dayBookings = await _bookingApiService.GetRoomCalendarAsync(id, selectedDate.AddDays(i), token);
                allBookings.AddRange(dayBookings);
            }

            ViewBag.Calendar = allBookings.DistinctBy(b => b.Id).ToList();
            ViewBag.Today = selectedDate;
            return View(room);
        }
        catch
        {
            return NotFound();
        }
    }
}