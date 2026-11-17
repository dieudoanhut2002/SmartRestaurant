# ==============================================================================
# SmartRestaurant — Windows Defender Firewall Configuration Script
# Opens inbound TCP ports 5000, 58329, 5098, 5099 for LAN Multi-Terminal Access
# Must be executed in an Elevated PowerShell (Run as Administrator)
# ==============================================================================

# Kiểm tra quyền Administrator
`$isAdmin = ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
if (-not `$isAdmin) {
    Write-Warning "Tập lệnh này yêu cầu quyền Administrator để cấu hình Windows Firewall."
    Write-Host "Vui lòng mở PowerShell bằng 'Run as Administrator' và chạy lại script này." -ForegroundColor Yellow
    exit 1
}

Write-Host "`n================================================================================" -ForegroundColor Cyan
Write-Host " SMARTRESTAURANT — CẤU HÌNH TƯỜNG LỬA WINDOWS DEFENDER CHO MẠNG LAN" -ForegroundColor Cyan
Write-Host "================================================================================`n" -ForegroundColor Cyan

`$rules = @(
    @{ Name = "SmartRestaurant-Manager-5000"; Port = 5000;  Desc = "SmartRestaurant Web Manager Portal (Port 5000)" },
    @{ Name = "SmartRestaurant-POS-58329";    Port = 58329; Desc = "SmartRestaurant Waiter Tablet POS (Port 58329)" },
    @{ Name = "SmartRestaurant-KDS-5098";     Port = 5098;  Desc = "SmartRestaurant Kitchen Display System (Port 5098)" },
    @{ Name = "SmartRestaurant-Cashier-5099"; Port = 5099;  Desc = "SmartRestaurant Cashier Terminal & VietQR (Port 5099)" }
)

foreach (`$r in `$rules) {
    Write-Host "-> Đang kiểm tra quy tắc: $($r.Name) (Cổng $($r.Port))..." -ForegroundColor Yellow
    
    # Xóa quy tắc cũ nếu đã tồn tại
    Get-NetFirewallRule -DisplayName `$r.Name -ErrorAction SilentlyContinue | Remove-NetFirewallRule
    
    # Tạo quy tắc mới cho phép Inbound TCP
    New-NetFirewallRule -DisplayName `$r.Name `
                        -Description `$r.Desc `
                        -Direction Inbound `
                        -LocalPort `$r.Port `
                        -Protocol TCP `
                        -Action Allow `
                        -Profile Domain, Private `
                        -Enabled True | Out-Null
                        
    Write-Host "  -> [ĐÃ MỞ] Cổng TCP $($r.Port) ($($r.Desc))" -ForegroundColor Green
}

Write-Host "`n================================================================================" -ForegroundColor Green
Write-Host " HOÀN TẤT: Cả 4 cổng mạng đã được mở thành công trên tường lửa!" -ForegroundColor Green
Write-Host " Giờ đây máy tính bảng Waiter và màn hình Bếp có thể kết nối thông qua Wi-Fi nội bộ." -ForegroundColor Green
Write-Host "================================================================================`n" -ForegroundColor Green
