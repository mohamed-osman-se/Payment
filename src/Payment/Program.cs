using System.Globalization;
using Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Payment.Config;
using Payment.Interfaces;
using Payment.Services;
using Stripe;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();

builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/login";
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                    options.SlidingExpiration = true;
                    options.Cookie.HttpOnly = true;
                    options.Cookie.Name = "Payment.Auth";
                });



builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite("Data Source =app.db"));


builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

builder.Services.AddSingleton(sp =>
{
    var opts = sp.GetRequiredService<IOptions<StripeSettings>>().Value;
    return new StripeClient(opts.SecretKey);
});


builder.Services.AddScoped(sp => new PaymentIntentService(sp.GetRequiredService<StripeClient>()));


builder.Services.AddScoped<IPaymentService, StripePaymentService>();
var cultureInfo = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
var app = builder.Build();

app.MapGet("/stripe/test", async () =>
{
    var service = new PaymentIntentService();
    var list = await service.ListAsync(new PaymentIntentListOptions { Limit = 1 });
    return Results.Ok(new { Connected = true, SampleCount = list.Data.Count });
});
app.UseHttpsRedirection();
app.UseExceptionHandler("/Home/Error");
app.UseStatusCodePagesWithReExecute("/Home/Error");
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Products}/{action=list}/{id?}")
    .WithStaticAssets();


app.Run();
