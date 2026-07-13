using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MeetSpot.Web.Controllers;

public class BaseController : Controller
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var token = Request.Cookies["AccessToken"];
        if (!string.IsNullOrEmpty(token))
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);

                var fullName = jwt.Claims.FirstOrDefault(c => c.Type == "FullName")?.Value;
                var email = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                var role = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                var userType = jwt.Claims.FirstOrDefault(c => c.Type == "UserType")?.Value;
                var avatarUrl = jwt.Claims.FirstOrDefault(c => c.Type == "AvatarUrl")?.Value;

                ViewBag.IsLoggedIn = true;
                ViewBag.FullName = fullName ?? email;
                ViewBag.UserEmail = email;
                ViewBag.UserRole = role;
                ViewBag.UserType = userType;
                ViewBag.AvatarUrl = avatarUrl;
            }
            catch
            {
                ViewBag.IsLoggedIn = false;
            }
        }
        else
        {
            ViewBag.IsLoggedIn = false;
        }

        base.OnActionExecuting(context);
    }
}