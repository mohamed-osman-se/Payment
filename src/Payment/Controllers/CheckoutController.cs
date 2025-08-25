using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payment.Interfaces;
[Authorize]
public class CheckoutController : Controller
{
    private readonly IPaymentService _paymentService;

    public CheckoutController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpGet]
    public IActionResult PayAll()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PayAllConfirmed()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var result = await _paymentService.StartCheckoutForUserAsync(userId);

        ViewData["ClientSecret"]   = result.ClientSecret;
        ViewData["PaymentIntentId"] = result.PaymentIntentId;
        ViewData["AmountMinor"]    = result.AmountMinor;
        ViewData["Currency"]       = result.Currency;
        ViewData["ItemCount"]      = result.Count; 

        ViewData["AmountMajor"] = (decimal)result.AmountMinor / 100;

        return View("PayAll");
    }

    [HttpGet]
    public IActionResult Complete(string payment_intent, string redirect_status)
    {
        ViewBag.PaymentIntentId = payment_intent;
        ViewBag.Status = redirect_status;
        return View();
    }
}
