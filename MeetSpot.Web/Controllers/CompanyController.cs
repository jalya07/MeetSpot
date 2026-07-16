using Microsoft.AspNetCore.Mvc;
using MeetSpot.Web.Services;

namespace MeetSpot.Web.Controllers;

public class CompanyController : BaseController
{
    private readonly CompanyApiService _companyApiService;
    private readonly CompanyRequestApiService _companyRequestApiService;

    public CompanyController(CompanyApiService companyApiService, CompanyRequestApiService companyRequestApiService)
    {
        _companyApiService = companyApiService;
        _companyRequestApiService = companyRequestApiService;
    }

    public async Task<IActionResult> Index()
    {
        var companies = await _companyApiService.GetAllCompaniesAsync();
        return View(companies);
    }

    [HttpGet]
    public IActionResult Apply() => View();

    [HttpPost]
    public async Task<IActionResult> Apply(string companyName, string domain,
        string contactEmail, string? industry, string? message)
    {
        var success = await _companyRequestApiService.SendRequestAsync(
            companyName, domain, contactEmail, industry, null, message);

        if (success)
            TempData["Success"] = "Your request has been submitted! We'll review it within 2 business days.";
        else
            TempData["Error"] = "Something went wrong. Please try again.";

        return RedirectToAction("Apply");
    }
}