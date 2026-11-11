namespace SmartRestaurant.Application.DTOs;

using SmartRestaurant.Domain.Enums;

public record StaffMemberDto(
    Guid Id,
    Guid BranchId,
    string FullName,
    string Username,
    string Email,
    StaffRole Role,
    string RoleName,
    bool IsActive);

public record CreateStaffRequest(
    Guid BranchId,
    string FullName,
    string Username,
    string Email,
    StaffRole Role,
    string PinCode = "1234");

public record UpdateStaffRequest(
    Guid StaffId,
    string FullName,
    string Email,
    StaffRole Role);

public record ResetPinRequest(
    Guid StaffId,
    string NewPinCode);

public record WorkShiftDto(
    Guid Id,
    Guid BranchId,
    Guid StaffMemberId,
    string StaffFullName,
    DateTime OpenedAt,
    DateTime? ClosedAt,
    decimal OpeningCashFloat,
    decimal SystemCashTotal,
    decimal? ActualCashCount,
    decimal? VarianceAmount,
    string Notes,
    ShiftStatus Status);

public record OpenShiftRequest(
    Guid BranchId,
    Guid StaffMemberId,
    decimal OpeningCashFloat,
    string Notes = "");

public record CloseShiftBlindRequest(
    Guid ShiftId,
    decimal ActualCashCount,
    string Notes = "");

public record ShiftVarianceLogDto(
    Guid Id,
    Guid ShiftId,
    decimal OpeningFloat,
    decimal SystemRevenue,
    decimal ExpectedAmount,
    decimal ActualCountedAmount,
    decimal VarianceAmount,
    string CashierNotes,
    DateTime LoggedAt);
