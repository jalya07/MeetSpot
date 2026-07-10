using Microsoft.AspNetCore.Mvc;
using MeetSpot.Web.Services;

namespace MeetSpot.Web.Controllers;

public class SubscriptionController : BaseController
{
    private readonly SubscriptionApiService _subscriptionApiService;
    private readonly UserApiService _userApiService;

    public SubscriptionController(SubscriptionApiService subscriptionApiService, UserApiService userApiService)
    {
        _subscriptionApiService = subscriptionApiService;
        _userApiService = userApiService;
    }

    public async Task<IActionResult> Index()
    {
        var token = Request.Cookies["AccessToken"];
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Account");

        var profile = await _userApiService.GetProfileAsync(token);
        var subscription = await _subscriptionApiService.GetMySubscriptionAsync(token);

        ViewBag.Profile = profile;
        ViewBag.Subscription = subscription;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Subscribe(string plan)
    {
        var token = Request.Cookies["AccessToken"];
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Account");

        var success = await _subscriptionApiService.CreateSubscriptionAsync(token, plan);
        if (success)
            TempData["Success"] = "Premium subscription activated!";
        else
            TempData["Error"] = "Could not activate subscription. Check your balance.";

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Cancel()
    {
        var token = Request.Cookies["AccessToken"];
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Account");

        await _subscriptionApiService.CancelSubscriptionAsync(token);
        TempData["Success"] = "Subscription cancelled.";
        return RedirectToAction("Index");
    }
}