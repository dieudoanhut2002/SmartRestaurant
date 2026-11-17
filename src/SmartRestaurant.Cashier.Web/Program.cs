using System.Globalization;
using Microsoft.AspNetCore.Localization;
using SmartRestaurant.Application;
using SmartRestaurant.Cashier.Web.Hubs;
using SmartRestaurant.Cashier.Web.Services;
using SmartRestaurant.Infrastructure;
using SmartRestaurant.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add Application & Infrastructure Services
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Add SignalR & Hosted Outbox Processor
builder.Services.AddSignalR();
builder.Services.AddHostedService<CashierOutboxProcessorHostedService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure Localization to Vietnamese (vi-VN per PDR-006 / BR-LOC-002)
var defaultCulture = new CultureInfo("vi-VN");
defaultCulture.NumberFormat.CurrencySymbol = "₫";
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(defaultCulture),
    SupportedCultures = [defaultCulture],
    SupportedUICultures = [defaultCulture]
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Cashier/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Map SignalR Cashier Hub
app.MapHub<CashierHub>("/hubs/cashier");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Cashier}/{action=Index}/{id?}");

// Auto-seed database on application startup if needed
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        await DatabaseSeeder.SeedAsync(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Đã xảy ra lỗi khi khởi tạo cơ sở dữ liệu cho Cashier.");
    }
}

app.Run();
