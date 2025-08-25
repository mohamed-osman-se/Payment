using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Payment.Config;
using Payment.Interfaces;
using Payment.Models;
using Stripe;

namespace Payment.Services
{
    public sealed class StripePaymentService : IPaymentService
    {

        private readonly AppDbContext _context;
        private readonly PaymentIntentService _paymentIntent;

        private readonly StripeSettings _opts;

        public StripePaymentService(AppDbContext context, PaymentIntentService paymentIntent, IOptions<StripeSettings> opts)
        {
            _context = context;
            _paymentIntent = paymentIntent;
            _opts = opts.Value;
        }

        public async Task<StartCheckoutResult> StartCheckoutForUserAsync(int userId, CancellationToken ct = default)
        {

            var orders = await _context.orders
            .Include(o => o.Product)
            .Where(o =>
             o.UserId == userId &&
            (o.Status == OrderStatus.Pending || o.Status == OrderStatus.AwaitingPayment))
            .ToListAsync(ct);

            if (orders.Count == 0)
                throw new InvalidOperationException();

            long totalMinor = 0;
            foreach (var o in orders)
            {
                var priceMinor = (long)Math.Round(o.Product!.Price * 100m, MidpointRounding.AwayFromZero);
                totalMinor += priceMinor;
            }


            var orderIdsSorted = orders.Select(x => x.Id).OrderBy(x => x).ToArray();
            var rawKey = $"payall:{userId}:{string.Join('-', orderIdsSorted)}:{totalMinor}:usd";
            var idemKey = ToStableKey(rawKey);



            var createOptions = new PaymentIntentCreateOptions
            {

                Currency = "usd",
                Amount = totalMinor,
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions { Enabled = true },
                Metadata = new Dictionary<string, string>
                {
                    ["user_id"] = userId.ToString(),
                    ["order_ids"] = string.Join(",", orderIdsSorted),
                    ["purpose"] = "pay_all_orders"
                }
            };

            var reqOpts = new RequestOptions { IdempotencyKey = idemKey };
            var pi = await _paymentIntent.CreateAsync(createOptions, reqOpts, ct);

            foreach (var o in orders)
            {
                o.PaymentIntentId = pi.Id;
                o.Status = OrderStatus.AwaitingPayment;
            }
            await _context.SaveChangesAsync(ct);

            return new StartCheckoutResult
            {
                PaymentIntentId = pi.Id,
                ClientSecret = pi.ClientSecret!,
                AmountMinor = totalMinor,
                Currency = "usd",
                Count = orders.Count
            };

        }

        private static string ToStableKey(string input)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }
    }
}