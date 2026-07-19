using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;

namespace MeetSpot.Web.Controllers;

public class BaseController : Controller
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        ViewBag.IsLoggedIn = false;

        var token = Request.Cookies["AccessToken"];
        if (!string.IsNullOrEmpty(token))
        {
            var config = HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var principal = ValidateToken(token, config);
            if (principal != null)
            {
                // Read claims tolerant of both the full-URI and short JWT forms,
                // since the mapping depends on the token writer's configuration.
                var email = Claim(principal, ClaimTypes.Email, "email");
                ViewBag.IsLoggedIn = true;
                ViewBag.UserEmail = email;
                ViewBag.FullName = Claim(principal, "FullName") ?? email;
                ViewBag.UserRole = Claim(principal, ClaimTypes.Role, "role");
                ViewBag.UserType = Claim(principal, "UserType");
                ViewBag.AvatarUrl = Claim(principal, "AvatarUrl");
            }
        }

        base.OnActionExecuting(context);
    }

    private static string? Claim(ClaimsPrincipal principal, params string[] types)
        => types.Select(t => principal.FindFirst(t)?.Value).FirstOrDefault(v => !string.IsNullOrEmpty(v));

    // Validates the JWT signature, issuer, audience and lifetime before trusting
    // any of its claims. Without this a forged cookie could render admin-only
    // pages/menus (the API itself always re-validates, but the UI must too).
    private static ClaimsPrincipal? ValidateToken(string token, IConfiguration config)
    {
        var key = config["Jwt:Key"];
        if (string.IsNullOrWhiteSpace(key))
            return null;

        try
        {
            var handler = new JwtSecurityTokenHandler { MapInboundClaims = false };
            var parameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = config["Jwt:Issuer"],
                ValidAudience = config["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                ClockSkew = TimeSpan.FromSeconds(30)
            };
            return handler.ValidateToken(token, parameters, out _);
        }
        catch
        {
            return null;
        }
    }
}
