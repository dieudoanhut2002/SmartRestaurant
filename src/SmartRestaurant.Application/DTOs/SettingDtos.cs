namespace SmartRestaurant.Application.DTOs;

public record RestaurantSettingDto(
    Guid BranchId,
    string BranchName,
    string Address,
    string PhoneNumber,
    TimeSpan BusinessDayCutoffTime,
    string FormattedCutoffTime,
    decimal VipRoomSurchargeRate,
    decimal VipRoomSurchargeFixedAmount,
    bool IsTableReservationEnabled,
    bool IsVietQrEnabled,
    bool IsInventoryEnabled,
    bool IsReportingEnabled
);

public record UpdateBusinessDayCutoffRequest(
    Guid BranchId,
    TimeSpan CutoffTime
);

public record UpdateVipSurchargeRequest(
    Guid BranchId,
    decimal SurchargeRate,
    decimal SurchargeFixedAmount
);

public record UpdateModuleConfigRequest(
    Guid BranchId,
    bool IsTableReservationEnabled,
    bool IsVietQrEnabled,
    bool IsInventoryEnabled,
    bool IsReportingEnabled
);

public record ApplyPresetRequest(
    Guid BranchId,
    string PresetName
);
