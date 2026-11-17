# ==============================================================================
# SmartRestaurant — Production Batch Publish Script
# Compiles and publishes all 4 operational hosts in Release mode to ./dist
# ==============================================================================

param(
    [string]$OutputDir = "$PSScriptRoot/../dist",
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"

Write-Host "`n================================================================================" -ForegroundColor Cyan
Write-Host " SMARTRESTAURANT — BẮT ĐẦU ĐÓNG GÓI BẢN PHÁT HÀNH (DOTNET PUBLISH)" -ForegroundColor Cyan
Write-Host " Cấu hình: $Configuration | Thư mục đích: $OutputDir" -ForegroundColor Cyan
Write-Host "================================================================================`n" -ForegroundColor Cyan

$root = Resolve-Path "$PSScriptRoot/.."
$dist = [System.IO.Path]::GetFullPath($OutputDir)

# Tạo thư mục đầu ra
if (Test-Path $dist) {
    Write-Host "-> Dọn dẹp thư mục dist cũ..." -ForegroundColor Gray
    Remove-Item -Path $dist -Recurse -Force
}
New-Item -ItemType Directory -Path $dist -Force | Out-Null

$projects = @(
    @{ Name = "Manager"; Path = "$root/src/SmartRestaurant.Web/SmartRestaurant.Web.csproj"; SubDir = "Manager" },
    @{ Name = "Pos";     Path = "$root/src/SmartRestaurant.Pos.Web/SmartRestaurant.Pos.Web.csproj"; SubDir = "Pos" },
    @{ Name = "Kds";     Path = "$root/src/SmartRestaurant.Kds.Web/SmartRestaurant.Kds.Web.csproj"; SubDir = "Kds" },
    @{ Name = "Cashier"; Path = "$root/src/SmartRestaurant.Cashier.Web/SmartRestaurant.Cashier.Web.csproj"; SubDir = "Cashier" }
)

$step = 1
foreach ($p in $projects) {
    $target = Join-Path $dist $p.SubDir
    Write-Host "[$step/4] Đang đóng gói $($p.Name) -> $target..." -ForegroundColor Yellow
    dotnet publish $p.Path -c $Configuration -o $target --nologo /p:UseAppHost=true
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Lỗi khi đóng gói $($p.Name)!"
        exit 1
    }
    Write-Host "  -> Thành công: $($p.Name)" -ForegroundColor Green
    $step++
}

# Tạo tệp launcher khởi chạy trực tiếp từ thư mục dist
$runnerContent = @"
# ==============================================================================
# SmartRestaurant — One-Click Production Launcher from Binary Package
# ==============================================================================
`$ip = (Get-NetIPAddress -AddressFamily IPv4 | Where-Object { `$_.InterfaceAlias -notlike "*Loopback*" -and `$_.IPAddress -like "192.168.*" } | Select-Object -First 1).IPAddress
if (-not `$ip) {
    `$ip = (Get-NetIPAddress -AddressFamily IPv4 | Where-Object { `$_.InterfaceAlias -notlike "*Loopback*" } | Select-Object -First 1).IPAddress
}
if (-not `$ip) { `$ip = "127.0.0.1" }

Write-Host "================================================================================" -ForegroundColor Cyan
Write-Host " SMARTRESTAURANT PRODUCTION RUNNER (LAN IP: `$ip)" -ForegroundColor Cyan
Write-Host "================================================================================" -ForegroundColor Cyan

Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd `"`$PSScriptRoot/Manager`"; .\SmartRestaurant.Web.exe --urls http://0.0.0.0:5000"
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd `"`$PSScriptRoot/Pos`"; .\SmartRestaurant.Pos.Web.exe --urls http://0.0.0.0:58329"
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd `"`$PSScriptRoot/Kds`"; .\SmartRestaurant.Kds.Web.exe --urls http://0.0.0.0:5098"
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd `"`$PSScriptRoot/Cashier`"; .\SmartRestaurant.Cashier.Web.exe --urls http://0.0.0.0:5099"

Write-Host "`nĐã kích hoạt 4 phân hệ nền tảng Kestrel trên toàn mạng LAN:" -ForegroundColor Green
Write-Host "  [1] Quản Lý (Manager):   http://`$(`$ip):5000" -ForegroundColor White
Write-Host "  [2] Bồi Bàn (POS):       http://`$(`$ip):58329/Floor" -ForegroundColor White
Write-Host "  [3] Màn Hình Bếp (KDS):  http://`$(`$ip):5098/Kds" -ForegroundColor White
Write-Host "  [4] Thu Ngân (Cashier):  http://`$(`$ip):5099/Cashier" -ForegroundColor White
"@

Set-Content -Path (Join-Path $dist "run-dist.ps1") -Value $runnerContent -Encoding UTF8

Write-Host "`n================================================================================" -ForegroundColor Green
Write-Host " HOÀN TẤT ĐÓNG GÓI 4 PHÂN HỆ THÀNH CÔNG VÀO THƯ MỤC: $dist" -ForegroundColor Green
Write-Host "================================================================================`n" -ForegroundColor Green
