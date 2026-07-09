using Microsoft.AspNetCore.Mvc;
using MeetSpot.Web.Services;

namespace MeetSpot.Web.Controllers;

public class BalanceController : BaseController
{
    private readonly UserApiService _userApiService;
    private readonly BalanceApiService _balanceApiService;

    public BalanceController(UserApiService userApiService, BalanceApiService balanceApiService)
    {
        _userApiService = userApiService;
        _balanceApiService = balanceApiService;
    }

    public async Task<IActionResult> Index()
    {
        var token = Request.Cookies["AccessToken"];
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Account");

        var profile = await _userApiService.GetProfileAsync(token);
        var transactions = await _balanceApiService.GetTransactionsAsync(token);

        ViewBag.Profile = profile;
        ViewBag.Transactions = transactions;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> TopUp(decimal amount, string cardNumber, string cardHolderName, string expiryMonth, string expiryYear, string cvv)
    {
        var token = Request.Cookies["AccessToken"];
        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Account");

        var expiryDate = $"{expiryMonth}/{expiryYear}";
        var success = await _balanceApiService.TopUpAsync(token, amount, cardNumber, cardHolderName, expiryDate, cvv);

        if (success)
            TempData["Success"] = $"${amount} added to your balance!";
        else
            TempData["Error"] = "Payment failed. Please try again.";

        return RedirectToAction("Index");
    }
}