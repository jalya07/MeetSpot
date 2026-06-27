using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MeetSpot.Core.DTOs.Analytics;
using MeetSpot.Core.Helpers;
using MeetSpot.Core.Interfaces;

namespace MeetSpot.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnalyticsController(IAnalyticsService analyticsService) : ControllerBase
{
    [HttpGet("dashboard")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ResponseModelHelper<AnalyticsDto>), 200)]
    public async Task<IActionResult> GetDashboard()
    {
        var result = await analyticsService.GetDashboardAnalyticsAsync();
        return Ok(ResponseModelHelper<AnalyticsDto>.SuccessResult(result));
    }

    [HttpGet("public-stats")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ResponseModelHelper<PublicStatsDto>), 200)]
    public async Task<IActionResult> GetPublicStats()
    {
        var result = await analyticsService.GetPublicStatsAsync();
        return Ok(ResponseModelHelper<PublicStatsDto>.SuccessResult(result));
    }
}