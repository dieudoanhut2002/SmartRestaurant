# ==============================================================================
# SmartRestaurant — Production / Pilot Store Multi-Terminal Launcher
# Binds all 4 services to 0.0.0.0 so tablets and kitchen displays on LAN can connect
# ==============================================================================

$root = Resolve-Path "$PSScriptRoot/.."

# Phát hiện địa chỉ IPv4 LAN
$ips = Get-NetIPAddress -AddressFamily IPv4 | Where-Object { $_.InterfaceAlias -notlike "*Loopback*" -and $_.IPAddress -notlike "169.254.*" } | Select-Object -ExpandProperty IPAddress
$preferredIp = $ips | Where-Object { $_ -like "192.168.*" } | Select-Object -First 1
if (-not $preferredIp) {
    $preferredIp = $ips | Where-Object { $_ -like "10.*" } | Select-Object -First 1
}
if (-not $preferredIp) {
    $preferredIp = $ips | Select-Object -First 1
}
if (-not $preferredIp) {
    $preferredIp = "127.0.0.1"
}

Write-Host "`n================================================================================" -ForegroundColor Cyan
Write-Host " SMARTRESTAURANT — KHỞI CHẠY HỆ THỐNG VẬN HÀNH THỬ NGHIỆM (PILOT STORE)" -ForegroundColor Cyan
Write-Host " Địa chỉ IP Máy chủ trong mạng Wi-Fi/LAN: $preferredIp" -ForegroundColor Yellow
Write-Host "================================================================================" -ForegroundColor Cyan

# 1. Manager Portal
Write-Host "-> [1/4] Khởi chạy Manager Web Portal tại http://0.0.0.0:5000..." -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd `"$root`"; dotnet run --project src/SmartRestaurant.Web/SmartRestaurant.Web.csproj --urls http://0.0.0.0:5000"

# 2. Waiter Tablet POS
Write-Host "-> [2/4] Khởi chạy POS Waiter Tablet tại http://0.0.0.0:58329..." -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd `"$root`"; dotnet run --project src/SmartRestaurant.Pos.Web/SmartRestaurant.Pos.Web.csproj --urls http://0.0.0.0:58329"

# 3. Kitchen Display System (KDS)
Write-Host "-> [3/4] Khởi chạy KDS Kitchen Display tại http://0.0.0.0:5098..." -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd `"$root`"; dotnet run --project src/SmartRestaurant.Kds.Web/SmartRestaurant.Kds.Web.csproj --urls http://0.0.0.0:5098"

# 4. Cashier Terminal & VietQR
Write-Host "-> [4/4] Khởi chạy Cashier Terminal & VietQR tại http://0.0.0.0:5099..." -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd `"$root`"; dotnet run --project src/SmartRestaurant.Cashier.Web/SmartRestaurant.Cashier.Web.csproj --urls http://0.0.0.0:5099"

Write-Host "`n================================================================================" -ForegroundColor Green
Write-Host " CẢ 4 PHÂN HỆ ĐÃ ĐƯỢC KHỞI CHẠY THÀNH CÔNG VỚI BINDING 0.0.0.0!" -ForegroundColor Green
Write-Host "================================================================================" -ForegroundColor Green
Write-Host " CÁC THIẾT BỊ TRONG NHÀ HÀNG (MẠNG WI-FI NỘI BỘ) TRUY CẬP THEO ĐƯỜNG DẪN SAU:" -ForegroundColor White
Write-Host ""
Write-Host " 📱 [1] Máy Tính Bảng Bồi Bàn (iPad/Android):" -ForegroundColor Cyan
Write-Host "      http://$($preferredIp):58329/Floor" -ForegroundColor Yellow
Write-Host ""
Write-Host " 🍳 [2] Màn Hình Bếp & Quầy Bar (Smart TV / KDS Tablet):" -ForegroundColor Cyan
Write-Host "      http://$($preferredIp):5098/Kds" -ForegroundColor Yellow
Write-Host ""
Write-Host " 💵 [3] Máy Thu Ngân & Xuất Phiếu Thanh Toán:" -ForegroundColor Cyan
Write-Host "      http://localhost:5099/Cashier  (hoặc http://$($preferredIp):5099/Cashier)" -ForegroundColor Yellow
Write-Host ""
Write-Host " 💻 [4] Quản Lý Nhà Hàng & Bản Quyền:" -ForegroundColor Cyan
Write-Host "      http://localhost:5000  (hoặc http://$($preferredIp):5000)" -ForegroundColor Yellow
Write-Host ""
Write-Host "--------------------------------------------------------------------------------" -ForegroundColor Gray
Write-Host " TÀI KHOẢN & MÃ PIN ĐĂNG NHẬP MẪU:" -ForegroundColor White
Write-Host "   - Quản Lý (Manager):   PIN: 1234  | manager@smartrestaurant.vn" -ForegroundColor White
Write-Host "   - Thu Ngân (Cashier):  PIN: 1111  | cashier@smartrestaurant.vn" -ForegroundColor White
Write-Host "   - Bồi Bàn (Waiter):    PIN: 2222  | waiter@smartrestaurant.vn" -ForegroundColor White
Write-Host "   - Bếp Trưởng (Chef):   PIN: 3333  | chef@smartrestaurant.vn" -ForegroundColor White
Write-Host "   - Quản Trị (Admin):    PIN: 9999  | admin@smartrestaurant.vn" -ForegroundColor White
Write-Host "================================================================================`n" -ForegroundColor Green
