using System;
using System.Threading.Tasks;
using Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Payment.Config;
using Payment.Models;
using Payment.Services;
using Stripe;
using Xunit;

public class StripePaymentServiceTests
{
  
    private static AppDbContext CreateContext(out SqliteConnection conn)
    {
        conn = new SqliteConnection("DataSource=:memory:");
        conn.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(conn)
            .Options;

        var ctx = new AppDbContext(options);
        ctx.Database.EnsureCreated();
        return ctx;
    }

    [Fact]
    public async Task StartCheckout_Throws_When_NoOrders()
    {
       
        var ctx = CreateContext(out var conn);

        var fakePaymentIntentService = new PaymentIntentServiceFake();

        var opts = Options.Create(new StripeSettings { SecretKey = "sk_test_123" });

        var service = new StripePaymentService(ctx, fakePaymentIntentService, opts);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.StartCheckoutForUserAsync(userId: 1));

        conn.Dispose();
    }
}



public class PaymentIntentServiceFake : PaymentIntentService
{
    public override Task<PaymentIntent> CreateAsync(PaymentIntentCreateOptions options, RequestOptions requestOptions = null!, System.Threading.CancellationToken cancellationToken = default)
    {
        var pi = new PaymentIntent
        {
            Id = "pi_test_123",
            ClientSecret = "secret_123"
        };
        return Task.FromResult(pi);
    }
}
