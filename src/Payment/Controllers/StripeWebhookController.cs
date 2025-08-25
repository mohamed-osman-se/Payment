using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Payment.Config;
using Payment.Models;
using Stripe;
using System.IO;

[ApiController]
[Route("webhooks/stripe")]
public class StripeWebhookController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ILogger<StripeWebhookController> _logger;
    private readonly StripeSettings _opts;

    public StripeWebhookController(AppDbContext db, ILogger<StripeWebhookController> logger, IOptions<StripeSettings> opts)
    {
        _db = db;
        _logger = logger;
        _opts = opts.Value;
    }

    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Handle()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        var signature = Request.Headers["Stripe-Signature"];

        Event stripeEvent;
        try
        {
            stripeEvent = EventUtility.ConstructEvent(json, signature, _opts.WebhookSecret);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "⚠️ Stripe signature verification failed.");
            return Unauthorized();
        }

        if (await _db.StripeEvents.AnyAsync(x => x.EventId == stripeEvent.Id))
            return Ok();

        _db.StripeEvents.Add(new StripeEventLog
        {
            EventId = stripeEvent.Id,
            Type = stripeEvent.Type,
            RawJson = json,
            ReceivedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();
        try
        {
            switch (stripeEvent.Type)
            {
                case "payment_intent.succeeded":
                {
                    var pi = stripeEvent.Data.Object as PaymentIntent;
                    if (pi != null)
                        await UpdateOrdersStatus(pi.Id, OrderStatus.Paid);
                    break;
                }
                case "payment_intent.payment_failed":
                {
                    var pi = stripeEvent.Data.Object as PaymentIntent;
                    if (pi != null)
                        await UpdateOrdersStatus(pi.Id, OrderStatus.Pending);
                    break;
                }
                case "payment_intent.canceled":
                {
                    var pi = stripeEvent.Data.Object as PaymentIntent;
                    if (pi != null)
                        await UpdateOrdersStatus(pi.Id, OrderStatus.Cancelled);
                    break;
                }
                case "charge.refunded":
                {
                    var charge = stripeEvent.Data.Object as Charge;
                    if (charge != null)
                        await UpdateOrdersStatus(charge.PaymentIntentId, OrderStatus.Cancelled);
                    break;
                }
                default:
                    _logger.LogInformation("Unhandled event type: {Type}", stripeEvent.Type);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, " Error processing event {EventId}", stripeEvent.Id);
            return StatusCode(500);
        }

        return Ok();
    }

    private async Task UpdateOrdersStatus(string paymentIntentId, OrderStatus status)
    {
        var orders = await _db.orders
            .Where(o => o.PaymentIntentId == paymentIntentId)
            .ToListAsync();

        foreach (var o in orders)
            o.Status = status;

        await _db.SaveChangesAsync();
    }
}
