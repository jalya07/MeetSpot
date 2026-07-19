using Microsoft.AspNetCore.Mvc;
using MeetSpot.Web.Services;

namespace MeetSpot.Web.Controllers;

public class CompanyController : BaseController
{
    private readonly IConfiguration _configuration;
    private readonly CompanyApiService _companyApiService;
    private readonly CompanyRequestApiService _companyRequestApiService;

    public CompanyController(IConfiguration configuration, CompanyApiService companyApiService, CompanyRequestApiService companyRequestApiService)
    {
        _configuration = configuration;
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
        string contactEmail, string? industry, string? message, IFormFile? logoFile)
    {
        string? logoUrl = null;

        if (logoFile != null && logoFile.Length > 0)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(logoFile.FileName).ToLower();
            if (allowedExtensions.Contains(extension))
            {
                var fileName = $"logo_{Guid.NewGuid()}{extension}";
                // Uploads root is the shared images volume (same volume the API
                // serves from). Save under the /logos subfolder so the file path
                // and the public URL agree.
                var uploadsRoot = _configuration["FileStorage:UploadsPath"] ?? Path.Combine(
                    Directory.GetCurrentDirectory(), "..", "MeetSpot.API", "wwwroot", "images");
                var uploadsPath = Path.Combine(uploadsRoot, "logos");
                Directory.CreateDirectory(uploadsPath);
                var filePath = Path.Combine(uploadsPath, fileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await logoFile.CopyToAsync(stream);
                logoUrl = $"/images/logos/{fileName}";
            }
        }

        var success = await _companyRequestApiService.SendRequestAsync(
            companyName, domain, contactEmail, industry, logoUrl, message);

        if (success)
            TempData["Success"] = "Your request has been submitted! We'll review it within 2 business days.";
        else
            TempData["Error"] = "Something went wrong. Please try again.";

        return RedirectToAction("Apply");
    }
}