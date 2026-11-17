using System.Globalization;
using Microsoft.AspNetCore.Localization;
using SmartRestaurant.Application;
using SmartRestaurant.Infrastructure;
using SmartRestaurant.Infrastructure.Data;
using SmartRestaurant.Pos.Web.Hubs;
using SmartRestaurant.Pos.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add Application & Infrastructure Services
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Add SignalR & Hosted Outbox Processor
builder.Services.AddSignalR();
builder.Services.AddHostedService<OutboxProcessorHostedService>();

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
    app.UseExceptionHandler("/Floor/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Map SignalR Hubs in Presentation Layer
app.MapHub<TableHub>("/hubs/table");
app.MapHub<KitchenHub>("/hubs/kitchen");
app.MapHub<PosOrderHub>("/hubs/pos-order");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Floor}/{action=Index}/{id?}");

// Auto-seed database on application startup
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
        logger.LogError(ex, "Đã xảy ra lỗi khi khởi tạo cơ sở dữ liệu cho POS.");
    }
}

app.Run();
