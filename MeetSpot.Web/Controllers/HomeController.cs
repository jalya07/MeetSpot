using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MeetSpot.Web.Models;
using MeetSpot.Web.Services;

namespace MeetSpot.Web.Controllers;

public class HomeController : BaseController
{
    private readonly RoomApiService _roomApiService;
    private readonly AnalyticsApiService _analyticsApiService;
    private readonly CompanyApiService _companyApiService;
    private readonly ReviewApiService _reviewApiService;

    public HomeController(RoomApiService roomApiService, AnalyticsApiService analyticsApiService, CompanyApiService companyApiService, ReviewApiService reviewApiService)
    {
        _roomApiService = roomApiService;
        _analyticsApiService = analyticsApiService;
        _companyApiService = companyApiService;
        _reviewApiService = reviewApiService;
    }

    public async Task<IActionResult> Index()
    {
        var rooms = await _roomApiService.GetAllRoomsAsync();
        var stats = await _analyticsApiService.GetPublicStatsAsync();
        var companies = await _companyApiService.GetAllCompaniesAsync();
        var reviews = await _reviewApiService.GetFeaturedAsync();

        ViewBag.Stats = stats;
        ViewBag.Companies = companies;
        ViewBag.Reviews = reviews;
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
