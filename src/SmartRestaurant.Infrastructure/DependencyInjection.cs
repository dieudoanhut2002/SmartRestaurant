namespace SmartRestaurant.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartRestaurant.Application.Common.Interfaces;
using SmartRestaurant.Application.DTOs;
using SmartRestaurant.Application.Interfaces;
using SmartRestaurant.Infrastructure.Data;
using SmartRestaurant.Infrastructure.Licensing;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString) || connectionString.Contains("InMemory", StringComparison.OrdinalIgnoreCase))
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase("SmartRestaurantDb"));
        }
        else
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                    sqlOptions.EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), null);
                }));
        }

        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

        // Cấu hình Nền tảng Bản Quyền (License Foundation - Owner Directive)
        services.Configure<LicenseOptions>(opts =>
        {
            var section = configuration.GetSection(LicenseOptions.SectionName);
            opts.Mode = section["Mode"] ?? "Development";
            opts.ProductCode = section["ProductCode"] ?? "SmartRestaurant";
            opts.IssuerPublicKeyPem = section["IssuerPublicKeyPem"];
            if (int.TryParse(section["DefaultOfflineAllowanceDays"], out var offline)) opts.DefaultOfflineAllowanceDays = offline;
            if (int.TryParse(section["DefaultExpiryGraceDays"], out var grace)) opts.DefaultExpiryGraceDays = grace;
            if (int.TryParse(section["ExpiringSoonDays"], out var soon)) opts.ExpiringSoonDays = soon;
            if (int.TryParse(section["ClockDriftToleranceMinutes"], out var drift)) opts.ClockDriftToleranceMinutes = drift;
            opts.StorageDirectory = section["StorageDirectory"] ?? string.Empty;
        });

        // Đăng ký các dịch vụ thành phần của Nền tảng Bản Quyền
        services.AddSingleton<ILicenseClockService, LicenseClockService>();
        services.AddSingleton<ILicenseSignatureValidator, LicenseSignatureValidator>();
        services.AddSingleton<IInstallationIdentityService, InstallationIdentityService>();
        services.AddSingleton<ILicenseStorageService, LicenseStorageService>();
        services.AddScoped<ILicenseService, LicenseService>();

        return services;
    }
}
