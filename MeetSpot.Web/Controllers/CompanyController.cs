using Microsoft.AspNetCore.Mvc;
using MeetSpot.Web.Services;

namespace MeetSpot.Web.Controllers;

public class CompanyController : BaseController
{
    private readonly CompanyApiService _companyApiService;

    public CompanyController(CompanyApiService companyApiService)
    {
        _companyApiService = companyApiService;
    }

    public async Task<IActionResult> Index()
    {
        var companies = await _companyApiService.GetAllCompaniesAsync();
        return View(companies);
    }
}