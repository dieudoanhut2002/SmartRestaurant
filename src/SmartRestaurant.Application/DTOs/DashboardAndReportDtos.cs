namespace SmartRestaurant.Application.DTOs;

public record DashboardStatsDto(
    decimal TodayRevenue,
    int ActiveTablesCount,
    int TotalTablesCount,
    double TableOccupancyRate,
    int OpenShiftsCount,
    int KitchenPendingTickets,
    int LowStockItemCount,
    List<HourlyRevenueDto> HourlyRevenue,
    List<TopSellingDishDto> TopSellingDishes,
    List<RecentActivityDto> RecentActivities);

public record HourlyRevenueDto(
    string HourLabel,
    decimal Revenue);

public record TopSellingDishDto(
    string DishCode,
    string DishName,
    string CategoryName,
    int QuantitySold,
    decimal TotalRevenue);

public record RecentActivityDto(
    DateTime Timestamp,
    string ActivityType,
    string Description,
    string PerformedBy);

public record DailySalesReportDto(
    DateTime BusinessDate,
    decimal TotalRevenue,
    decimal CashRevenue,
    decimal VietQrRevenue,
    decimal CardRevenue,
    int TotalOrders,
    decimal AverageTicketSize,
    List<CategorySalesBreakdownDto> CategoryBreakdown);

public record CategorySalesBreakdownDto(
    string CategoryName,
    int ItemCount,
    decimal Revenue,
    double Percentage);

public record KitchenWasteLogDto(
    Guid Id,
    Guid MenuItemId,
    string MenuItemName,
    int Quantity,
    string Reason,
    decimal EstimatedLoss,
    string StaffName,
    DateTime LoggedAt);

public record ManagerPinAuditLogDto(
    Guid Id,
    Guid ManagerStaffId,
    string ManagerName,
    string ActionName,
    string TargetEntity,
    string Reason,
    DateTime Timestamp);

public record HourlySalesAnalyticsDto(
    int Hour,
    string HourLabel,
    decimal Revenue,
    int OrderCount,
    int GuestCount,
    string PeakLevel);

public record TableTurnoverMetricDto(
    Guid TableId,
    string TableCode,
    string TableName,
    string AreaName,
    int TurnsCount,
    double AverageDurationMinutes,
    decimal TotalRevenueGenerated);

public record KdsSlaPerformanceDto(
    int TotalTickets,
    int FastUnder10MinCount,
    int Medium10To20MinCount,
    int Overdue20MinCount,
    double SlaCompliancePercent,
    double AverageElapsedMinutes);

public record MenuEngineeringItemDto(
    Guid MenuItemId,
    string DishCode,
    string DishName,
    string CategoryName,
    decimal SellingPrice,
    decimal EstimatedCost,
    decimal GrossMarginAmount,
    double MarginPercent,
    int QuantitySold,
    decimal TotalProfit,
    string Classification,
    string BadgeClass);

public record ExecutiveBiDashboardDto(
    DateTime FromDate,
    DateTime ToDate,
    decimal TotalRevenue,
    decimal TotalGrossProfit,
    double OverallMarginPercent,
    int TotalOrders,
    decimal AverageTicketSize,
    double AverageTableDurationMinutes,
    decimal CashRevenue,
    decimal VietQrRevenue,
    decimal CardRevenue,
    List<HourlySalesAnalyticsDto> HourlyHeatmap,
    List<TableTurnoverMetricDto> TopTables,
    KdsSlaPerformanceDto SlaPerformance,
    List<MenuEngineeringItemDto> MenuEngineering,
    List<CategorySalesBreakdownDto> CategoryBreakdown);

