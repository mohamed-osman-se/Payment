using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;




public sealed class StartCheckoutResult
{
    public string PaymentIntentId { get; init; } = null!;
    public string ClientSecret { get; init; } = null!;
    public long AmountMinor { get; init; }
    public string Currency { get; init; } = null!;

    public int? Count { get; init; } = null!;
}


namespace Payment.Interfaces
{
    public interface IPaymentService
    {
        Task<StartCheckoutResult> StartCheckoutForUserAsync(int userId, CancellationToken ct = default);
    }
}