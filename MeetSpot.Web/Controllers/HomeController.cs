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
    private readonly HeroSlideApiService _heroSlideApiService;

    public HomeController(
        RoomApiService roomApiService,
        AnalyticsApiService analyticsApiService,
        CompanyApiService companyApiService,
        ReviewApiService reviewApiService,
        HeroSlideApiService heroSlideApiService)
    {
        _roomApiService = roomApiService;
        _analyticsApiService = analyticsApiService;
        _companyApiService = companyApiService;
        _reviewApiService = reviewApiService;
        _heroSlideApiService = heroSlideApiService;
    }

    public async Task<IActionResult> Index()
    {
        var rooms = await _roomApiService.GetAllRoomsAsync();
        var stats = await _analyticsApiService.GetPublicStatsAsync();
        var companies = await _companyApiService.GetAllCompaniesAsync();
        var reviews = await _reviewApiService.GetFeaturedAsync();
        var heroSlides = await _heroSlideApiService.GetActiveSlidesAsync();

        ViewBag.Stats = stats;
        ViewBag.Companies = companies;
        ViewBag.Reviews = reviews;
        ViewBag.HeroSlides = heroSlides;

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

    public IActionResult Pricing()
    {
        return View();
    }

    public IActionResult About()
    {
        return View();
    }

    public IActionResult Contact()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Contact(string firstName, string lastName, string email, string subject, string message)
    {
        var token = Request.Cookies["AccessToken"];

        var contactApiService = HttpContext.RequestServices.GetRequiredService<ContactApiService>();
        await contactApiService.SendMessageAsync(firstName, lastName, email, subject, message);

        TempData["ContactSuccess"] = "Your message has been sent! We'll get back to you within 24 hours.";
        return RedirectToAction("Contact");
    }
}
