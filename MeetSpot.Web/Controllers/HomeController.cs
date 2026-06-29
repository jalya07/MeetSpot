using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MeetSpot.Web.Models;
using MeetSpot.Web.Services;

namespace MeetSpot.Web.Controllers;

public class HomeController : Controller
{
    private readonly RoomApiService _roomApiService;
    private readonly AnalyticsApiService _analyticsApiService;
    private readonly CompanyApiService _companyApiService;

    public HomeController(RoomApiService roomApiService, AnalyticsApiService analyticsApiService, CompanyApiService companyApiService)
    {
        _roomApiService = roomApiService;
        _analyticsApiService = analyticsApiService;
        _companyApiService = companyApiService;
    }

    public async Task<IActionResult> Index()
    {
        var rooms = await _roomApiService.GetAllRoomsAsync();
        var stats = await _analyticsApiService.GetPublicStatsAsync();
        var companies = await _companyApiService.GetAllCompaniesAsync();

        ViewBag.Stats = stats;
        ViewBag.Companies = companies;
        return View(rooms);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
