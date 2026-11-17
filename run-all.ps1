# ==============================================================================
# SmartRestaurant — One-Click Multi-Workstation Launch Script
# Launches Manager Portal, Waiter Tablet POS, Kitchen Display, and Cashier Terminal
# ==============================================================================

Write-Host "`n[SMARTRESTAURANT] Đang khởi chạy hệ thống 4 trạm làm việc..." -ForegroundColor Cyan

# 1. Manager Portal (Admin & Floor Plan Designer)
Write-Host "-> [1/4] Khởi chạy Manager Web Portal tại http://localhost:5000..." -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit", "-Command", "dotnet run --project src/SmartRestaurant.Web/SmartRestaurant.Web.csproj --urls http://localhost:5000"

# 2. Waiter Tablet POS
Write-Host "-> [2/4] Khởi chạy POS Waiter Tablet tại http://localhost:58329..." -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit", "-Command", "dotnet run --project src/SmartRestaurant.Pos.Web/SmartRestaurant.Pos.Web.csproj --urls http://localhost:58329"

# 3. Kitchen Display System (KDS & Expediter Pass)
Write-Host "-> [3/4] Khởi chạy KDS Kitchen Display tại http://localhost:5098..." -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit", "-Command", "dotnet run --project src/SmartRestaurant.Kds.Web/SmartRestaurant.Kds.Web.csproj --urls http://localhost:5098"

# 4. Cashier Terminal & VietQR Gateway
Write-Host "-> [4/4] Khởi chạy Cashier Terminal & VietQR tại http://localhost:5099..." -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit", "-Command", "dotnet run --project src/SmartRestaurant.Cashier.Web/SmartRestaurant.Cashier.Web.csproj --urls http://localhost:5099"

Write-Host "`n[SMARTRESTAURANT] Cả 4 phân hệ đã được khởi chạy trong các cửa sổ riêng biệt!" -ForegroundColor Green
Write-Host "  - Manager Portal:    http://localhost:5000"
Write-Host "  - Waiter POS:        http://localhost:58329/Floor"
Write-Host "  - Kitchen KDS:       http://localhost:5098/Kds"
Write-Host "  - Cashier Terminal:  http://localhost:5099/Cashier"
Write-Host "`nXem chi tiết tài khoản và hướng dẫn vận hành tại file HUONGDAN.txt`n"
