namespace SmartRestaurant.Domain.Entities;

using SmartRestaurant.Domain.Common;
using SmartRestaurant.Domain.Exceptions;

public class Branch : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public string Address { get; private set; } = string.Empty;
    public string PhoneNumber { get; private set; } = string.Empty;
    public TimeSpan BusinessDateCutoffTime { get; private set; } = new(6, 0, 0); // Default 06:00 per D1 / PDR-007
    public bool IsActive { get; private set; } = true;

    // VIP Room Surcharges (REV-06 / PDR-011)
    public decimal VipRoomDefaultSurchargeRate { get; private set; } = 10m; // 10%
    public decimal VipRoomDefaultFixedAmount { get; private set; } = 150000m; // 150,000 VND

    // Module Configuration Flags (10 Core Mandatory + 4 MVP Optional per OD-15, OD-16, OD-17)
    public bool IsTableReservationEnabled { get; private set; } = true; // MOD-06: Đặt bàn trước (OD-15)
    public bool IsVietQrEnabled { get; private set; } = true;           // MOD-11: Cổng VietQR động Napas 24/7
    public bool IsInventoryEnabled { get; private set; } = true;        // MOD-12: Quản lý kho hàng đếm được
    public bool IsReportingEnabled { get; private set; } = true;        // MOD-13: Báo cáo & Kiểm toán nâng cao

    public string CurrentOnboardingPreset { get; private set; } = "Alacarte"; // OD-16: Default À la carte Dining

    protected Branch() { }

    public Branch(string name, string address, string phoneNumber, TimeSpan? cutoffTime = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainValidationException("Tên chi nhánh không được để trống.");

        Name = name.Trim();
        Address = address?.Trim() ?? string.Empty;
        PhoneNumber = phoneNumber?.Trim() ?? string.Empty;
        BusinessDateCutoffTime = cutoffTime ?? new TimeSpan(6, 0, 0);
        IsActive = true;
    }

    public void UpdateInfo(string name, string address, string phoneNumber, TimeSpan cutoffTime)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainValidationException("Tên chi nhánh không được để trống.");

        Name = name.Trim();
        Address = address?.Trim() ?? string.Empty;
        PhoneNumber = phoneNumber?.Trim() ?? string.Empty;
        BusinessDateCutoffTime = cutoffTime;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateBusinessDayCutoff(TimeSpan cutoffTime)
    {
        BusinessDateCutoffTime = cutoffTime;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateVipSurcharges(decimal rate, decimal fixedAmount)
    {
        if (rate < 0 || rate > 100)
            throw new DomainValidationException("Tỷ lệ phụ thu VIP phải từ 0% đến 100%.");
        if (fixedAmount < 0)
            throw new DomainValidationException("Tiền phụ thu cố định không được âm.");

        VipRoomDefaultSurchargeRate = rate;
        VipRoomDefaultFixedAmount = fixedAmount;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ConfigureModules(bool reservation, bool vietQr, bool inventory, bool reporting)
    {
        IsTableReservationEnabled = reservation;
        IsVietQrEnabled = vietQr;
        IsInventoryEnabled = inventory;
        IsReportingEnabled = reporting;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ApplyOnboardingPreset(string presetName)
    {
        switch (presetName.Trim().ToLowerInvariant())
        {
            case "cafe":
                // Gói Cafe / Boutique: Tắt đặt bàn, tắt phụ thu VIP, bật VietQR & Kho
                CurrentOnboardingPreset = "Cafe";
                IsTableReservationEnabled = false;
                IsVietQrEnabled = true;
                IsInventoryEnabled = true;
                IsReportingEnabled = true;
                VipRoomDefaultSurchargeRate = 0;
                VipRoomDefaultFixedAmount = 0;
                break;

            case "pub":
            case "beerpub":
                // Gói Pub & Bia: Giờ cắt ca 06:00, tắt đặt bàn, tập trung kho hàng đếm được
                CurrentOnboardingPreset = "BeerPub";
                IsTableReservationEnabled = false;
                IsVietQrEnabled = true;
                IsInventoryEnabled = true;
                IsReportingEnabled = true;
                VipRoomDefaultSurchargeRate = 5m;
                VipRoomDefaultFixedAmount = 100000m;
                break;

            case "alacarte":
            default:
                // Gói À la carte truyền thống: Đầy đủ 4 module tùy chọn, có phụ thu VIP
                CurrentOnboardingPreset = "Alacarte";
                IsTableReservationEnabled = true;
                IsVietQrEnabled = true;
                IsInventoryEnabled = true;
                IsReportingEnabled = true;
                VipRoomDefaultSurchargeRate = 10m;
                VipRoomDefaultFixedAmount = 150000m;
                break;
        }

        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }
}
