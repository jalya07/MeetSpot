using Microsoft.AspNetCore.Mvc;
using MeetSpot.Web.Services;

namespace MeetSpot.Web.Controllers;

public class AccountController : Controller
{
    private readonly AuthApiService _authApiService;

    public AccountController(AuthApiService authApiService)
    {
        _authApiService = authApiService;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return View(request);
        }

        var (success, errors) = await _authApiService.RegisterAsync(request);

        if (success)
        {
            TempData["SuccessMessage"] = "Account created! Please check your email to confirm your account.";
            return RedirectToAction("Login");
        }

        foreach (var error in errors)
        {
            ModelState.AddModelError(string.Empty, error);
        }

        return View(request);
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return View(request);
        }

        var (success, requiresTwoFactor, errors) = await _authApiService.LoginAsync(request);

        if (success && requiresTwoFactor)
        {
            TempData["PendingEmail"] = request.Email;
            return RedirectToAction("VerifyTwoFactor");
        }

        foreach (var error in errors)
        {
            ModelState.AddModelError(string.Empty, error);
        }

        return View(request);
    }

    [HttpGet]
    public IActionResult VerifyTwoFactor()
    {
        var email = TempData["PendingEmail"] as string;
        if (string.IsNullOrEmpty(email))
        {
            return RedirectToAction("Login");
        }

        TempData.Keep("PendingEmail");
        ViewBag.Email = email;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> VerifyTwoFactor(string code)
    {
        var email = TempData["PendingEmail"] as string;
        if (string.IsNullOrEmpty(email))
        {
            return RedirectToAction("Login");
        }

        var request = new VerifyTwoFactorRequest { Email = email, Code = code };
        var (success, accessToken, refreshToken, errors) = await _authApiService.VerifyTwoFactorAsync(request);

        if (success)
        {
            Response.Cookies.Append("AccessToken", accessToken!, new CookieOptions { HttpOnly = true, Expires = DateTimeOffset.UtcNow.AddHours(1) });
            Response.Cookies.Append("RefreshToken", refreshToken!, new CookieOptions { HttpOnly = true, Expires = DateTimeOffset.UtcNow.AddDays(7) });

            return RedirectToAction("Index", "Home");
        }

        TempData.Keep("PendingEmail");
        ViewBag.Email = email;
        ViewBag.Error = errors.FirstOrDefault() ?? "Verification failed";
        return View();
    }
}