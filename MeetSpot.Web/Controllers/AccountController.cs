using Microsoft.AspNetCore.Mvc;
using MeetSpot.Web.Services;

namespace MeetSpot.Web.Controllers;

public class AccountController : BaseController
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
        if (request.Password != request.ConfirmPassword)
        {
            ModelState.AddModelError(string.Empty, "Passwords do not match");
            return View(request);
        }

        if (!ModelState.IsValid)
            return View(request);

        var (success, errors) = await _authApiService.RegisterAsync(request);

        if (success)
        {
            TempData["SuccessMessage"] = "Account created! Please check your email to confirm your account.";
            return RedirectToAction("Login");
        }

        foreach (var error in errors)
            ModelState.AddModelError(string.Empty, error);

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
            return View(request);

        var (success, requiresTwoFactor, accessToken, refreshToken, errors) = await _authApiService.LoginAsync(request);

        if (success && requiresTwoFactor)
        {
            TempData["PendingEmail"] = request.Email;
            return RedirectToAction("VerifyTwoFactor");
        }

        if (success && !requiresTwoFactor && accessToken != null)
        {
            Response.Cookies.Append("AccessToken", accessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Lax,
                // Outlives the 1h token so the expired token stays available for refresh.
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });
            Response.Cookies.Append("RefreshToken", refreshToken!, new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });
            return RedirectToAction("Index", "Home");
        }

        foreach (var error in errors)
            ModelState.AddModelError(string.Empty, error);

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
        TempData.Keep("IsAdminLogin");
        ViewBag.Email = email;
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> VerifyTwoFactor(string code)
    {
        var email = TempData["PendingEmail"] as string;

        if (string.IsNullOrEmpty(email))
            return RedirectToAction("Login");

        var request = new VerifyTwoFactorRequest { Email = email, Code = code };
        var (success, accessToken, refreshToken, errors) = await _authApiService.VerifyTwoFactorAsync(request);

        if (success)
        {
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(accessToken);
            var role = jwt.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;

            var adminCookie = Request.Cookies["AdminLoginAttempt"];
            Response.Cookies.Delete("AdminLoginAttempt");

        // Admin пытается войти через обычный login
            if (string.IsNullOrEmpty(adminCookie) && role == "Admin")
            {
                TempData["Error"] = "Please use the Admin login page.";
                return RedirectToAction("Login", "Account");
            }

        // Обычный пользователь пытается войти через Admin login
            if (!string.IsNullOrEmpty(adminCookie) && role != "Admin")
            {
                TempData["Error"] = "Access denied. Admin credentials required.";
                return RedirectToAction("Login", "Admin");
            }

            Response.Cookies.Append("AccessToken", accessToken!, new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Lax,
                // Outlives the 1h token so the expired token stays available for refresh.
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });
            Response.Cookies.Append("RefreshToken", refreshToken!, new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });

            if (!string.IsNullOrEmpty(adminCookie))
                return RedirectToAction("Dashboard", "Admin");

            return RedirectToAction("Index", "Home");
        }

        TempData["PendingEmail"] = email;
        ViewBag.Email = email;
        ViewBag.Error = errors.FirstOrDefault() ?? "Verification failed";
        return View();
    }

    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
    {
        var (success, errors) = await _authApiService.ForgotPasswordAsync(request);

        if (success)
        {
            TempData["SuccessMessage"] = "If this email exists, you'll receive a reset link shortly.";
            return RedirectToAction("ForgotPasswordConfirmation");
        }

        foreach (var error in errors)
            ModelState.AddModelError(string.Empty, error);

        return View(request);
    }

    [HttpGet]
    public IActionResult ForgotPasswordConfirmation()
    {
        return View();
    }

    [HttpGet]
    public IActionResult ResetPassword(string token, string email)
    {
        ViewBag.Token = token;
        ViewBag.Email = email;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
    {
        if (request.NewPassword != request.ConfirmPassword)
        {
            ModelState.AddModelError(string.Empty, "Passwords do not match");
            ViewBag.Token = request.Token;
            ViewBag.Email = request.Email;
            return View(request);
        }

        var (success, errors) = await _authApiService.ResetPasswordAsync(request);

        if (success)
        {
            TempData["SuccessMessage"] = "Password reset successfully! You can now log in.";
            return RedirectToAction("Login");
        }

        foreach (var error in errors)
            ModelState.AddModelError(string.Empty, error);

        ViewBag.Token = request.Token;
        ViewBag.Email = request.Email;
        return View(request);
    }

    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(string token, string email)
    {
        var (success, errors) = await _authApiService.ConfirmEmailAsync(token, email);

        if (success)
        {
            TempData["SuccessMessage"] = "Email confirmed successfully! You can now log in.";
            return RedirectToAction("Login");
        }

        ViewBag.Error = errors.FirstOrDefault() ?? "Confirmation failed";
        return View();
    }
    [HttpGet]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("AccessToken");
        Response.Cookies.Delete("RefreshToken");
        return RedirectToAction("Index", "Home");
        
    }
}