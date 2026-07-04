using Microsoft.AspNetCore.Mvc;
using MeetSpot.Web.Services;

namespace MeetSpot.Web.Controllers;

public class RoomController : BaseController
{
    private readonly RoomApiService _roomApiService;

    public RoomController(RoomApiService roomApiService)
    {
        _roomApiService = roomApiService;
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
            return View(room);
        }
        catch
        {
            return NotFound();
        }
    }
}