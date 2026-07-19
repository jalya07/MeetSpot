using System.IdentityModel.Tokens.Jwt;
using MeetSpot.Web.Services;

namespace MeetSpot.Web.Middleware;

// Transparently refreshes an expired (or nearly expired) access token using the
// refresh-token cookie, so a user holding a valid 7-day refresh token is not
// silently logged out when the 1-hour access token expires. All failures are
// swallowed so the request simply proceeds with the existing cookies.
public class TokenRefreshMiddleware
{
    private readonly RequestDelegate _next;

    public TokenRefreshMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, UserApiService userApi)
    {
        try
        {
            var accessToken = context.Request.Cookies["AccessToken"];
            var refreshToken = context.Request.Cookies["RefreshToken"];

            // The refresh endpoint needs the (possibly expired) access token to
            // identify the user, so both cookies must be present.
            if (!string.IsNullOrEmpty(accessToken)
                && !string.IsNullOrEmpty(refreshToken)
                && IsExpiredOrExpiring(accessToken))
            {
                var tokens = await userApi.RefreshTokenAsync(refreshToken, accessToken);
                if (tokens != null && !string.IsNullOrEmpty(tokens.AccessToken))
                {
                    var secure = context.Request.IsHttps;

                    // Keep the access cookie alive as long as the refresh cookie
                    // so the expired token remains available for the next refresh.
                    context.Response.Cookies.Append("AccessToken", tokens.AccessToken, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = secure,
                        SameSite = SameSiteMode.Lax,
                        Expires = DateTimeOffset.UtcNow.AddDays(7)
                    });
                    context.Response.Cookies.Append("RefreshToken", tokens.RefreshToken, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = secure,
                        SameSite = SameSiteMode.Lax,
                        Expires = DateTimeOffset.UtcNow.AddDays(7)
                    });

                    // Make the fresh token visible to controllers that read the
                    // request cookie later in this same request.
                    ReplaceRequestCookie(context, "AccessToken", tokens.AccessToken);
                }
            }
        }
        catch
        {
            // Fall through with existing cookies unchanged.
        }

        await _next(context);
    }

    private static bool IsExpiredOrExpiring(string accessToken)
    {
        try
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
            // Refresh if already expired or expiring within the next 2 minutes.
            return jwt.ValidTo <= DateTime.UtcNow.AddMinutes(2);
        }
        catch
        {
            // Unreadable token — treat as needing refresh.
            return true;
        }
    }

    private static void ReplaceRequestCookie(HttpContext context, string name, string value)
    {
        var cookies = context.Request.Cookies.ToDictionary(c => c.Key, c => c.Value);
        cookies[name] = value;
        var header = string.Join("; ", cookies.Select(kv => $"{kv.Key}={kv.Value}"));
        context.Request.Headers["Cookie"] = header;
    }
}
