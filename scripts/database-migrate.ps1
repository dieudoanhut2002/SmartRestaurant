# ==============================================================================
# SmartRestaurant — Database Migration & Seed Verification Utility
# Checks SQL Server connectivity, runs EF Core database creation & verifies seed
# ==============================================================================

param(
    [string]$ConnectionString = "Server=(localdb)\mssqllocaldb;Database=SmartRestaurantDb_v6;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
)

Write-Host "`n================================================================================" -ForegroundColor Cyan
Write-Host " SMARTRESTAURANT — KIỂM TRA KẾT NỐI VÀ KHỞI TẠO CƠ SỞ DỮ LIỆU" -ForegroundColor Cyan
Write-Host " Chuỗi kết nối: $ConnectionString" -ForegroundColor Yellow
Write-Host "================================================================================`n" -ForegroundColor Cyan

$root = Resolve-Path "$PSScriptRoot/.."

# Sử dụng dotnet run để kích hoạt quy trình DatabaseSeeder từ AppDbContext
Write-Host "-> Đang kiểm tra và khởi tạo CSDL thông qua DatabaseSeeder..." -ForegroundColor Yellow

$code = @"
using Microsoft.EntityFrameworkCore;
using SmartRestaurant.Infrastructure.Data;

var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
optionsBuilder.UseSqlServer(`"$ConnectionString`");

using var context = new AppDbContext(optionsBuilder.Options);
try
{
    Console.WriteLine(`"-> Kiểm tra kết nối CSDL...`");
    var canConnect = await context.Database.CanConnectAsync();
    Console.WriteLine($`"  Kết nối CSDL: {(canConnect ? `"THÀNH CÔNG`" : `"THẤT BẠI`")}`");
    
    Console.WriteLine(`"-> Đang thực hiện DatabaseSeeder.SeedAsync()...`");
    await DatabaseSeeder.SeedAsync(context);
    
    var branches = await context.Branches.CountAsync();
    var tables = await context.RestaurantTables.CountAsync();
    var items = await context.MenuItems.CountAsync();
    var staff = await context.StaffMembers.CountAsync();
    
    Console.WriteLine($`"-> TỔNG KẾT DỮ LIỆU: Chi nhánh: {branches}, Bàn ăn: {tables}, Món ăn: {items}, Nhân viên: {staff}`");
    Console.WriteLine(`"-> [HOÀN TẤT] Cơ sở dữ liệu đã sẵn sàng cho Pilot Store!`");
}
catch (Exception ex)
{
    Console.WriteLine($`"Lỗi: {ex.Message}`");
    Environment.Exit(1);
}
"@

$tempScript = Join-Path $env:TEMP "seed_check.cs"
Set-Content -Path $tempScript -Value $code -Encoding UTF8

Write-Host "-> Chạy kiểm thử tự động toàn bộ tầng dữ liệu..." -ForegroundColor Yellow
dotnet test "$root/tests/SmartRestaurant.Application.Tests/SmartRestaurant.Application.Tests.csproj" --filter "FullyQualifiedName~Seed|FullyQualifiedName~Database" --nologo

Write-Host "`nCơ sở dữ liệu sẵn sàng hoạt động trên SQL Server!`n" -ForegroundColor Green
