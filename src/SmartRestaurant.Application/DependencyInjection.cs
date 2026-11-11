namespace SmartRestaurant.Application;

using Microsoft.Extensions.DependencyInjection;
using SmartRestaurant.Application.Interfaces;
using SmartRestaurant.Application.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IFloorPlanService, FloorPlanService>();
        services.AddScoped<IMenuService, MenuService>();
        services.AddScoped<IStaffService, StaffService>();
        services.AddScoped<IShiftService, ShiftService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IKitchenStationService, KitchenStationService>();
        services.AddScoped<IInventoryService, InventoryService>();
        services.AddScoped<ISettingService, SettingService>();

        return services;
    }
}
