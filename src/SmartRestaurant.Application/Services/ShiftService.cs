namespace SmartRestaurant.Application.Services;

using Microsoft.EntityFrameworkCore;
using SmartRestaurant.Application.Common.Interfaces;
using SmartRestaurant.Application.DTOs;
using SmartRestaurant.Application.Interfaces;
using SmartRestaurant.Domain.Entities;
using SmartRestaurant.Domain.Enums;
using SmartRestaurant.Domain.Exceptions;

public class ShiftService(IAppDbContext dbContext) : IShiftService
{
    public async Task<List<WorkShiftDto>> GetActiveShiftsAsync(Guid branchId, CancellationToken ct = default)
    {
        var shifts = await dbContext.WorkShifts
            .Where(s => s.BranchId == branchId && s.Status == ShiftStatus.Open)
            .OrderByDescending(s => s.OpenedAt)
            .ToListAsync(ct);

        var staffIds = shifts.Select(s => s.StaffMemberId).Distinct().ToList();
        var staffDict = await dbContext.StaffMembers
            .Where(sm => staffIds.Contains(sm.Id))
            .ToDictionaryAsync(sm => sm.Id, sm => sm.FullName, ct);

        return shifts.Select(s => new WorkShiftDto(
            s.Id,
            s.BranchId,
            s.StaffMemberId,
            staffDict.TryGetValue(s.StaffMemberId, out var name) ? name : "Nhân viên",
            s.OpenedAt,
            s.ClosedAt,
            s.OpeningCashFloat,
            s.SystemCashTotal,
            s.ActualCashCount,
            s.VarianceAmount,
            s.Notes,
            s.Status)).ToList();
    }

    public async Task<List<WorkShiftDto>> GetShiftHistoryAsync(Guid branchId, int pageNumber = 1, int pageSize = 20, CancellationToken ct = default)
    {
        var shifts = await dbContext.WorkShifts
            .Where(s => s.BranchId == branchId)
            .OrderByDescending(s => s.OpenedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        var staffIds = shifts.Select(s => s.StaffMemberId).Distinct().ToList();
        var staffDict = await dbContext.StaffMembers
            .Where(sm => staffIds.Contains(sm.Id))
            .ToDictionaryAsync(sm => sm.Id, sm => sm.FullName, ct);

        return shifts.Select(s => new WorkShiftDto(
            s.Id,
            s.BranchId,
            s.StaffMemberId,
            staffDict.TryGetValue(s.StaffMemberId, out var name) ? name : "Nhân viên",
            s.OpenedAt,
            s.ClosedAt,
            s.OpeningCashFloat,
            s.SystemCashTotal,
            s.ActualCashCount,
            s.VarianceAmount,
            s.Notes,
            s.Status)).ToList();
    }

    public async Task<WorkShiftDto?> GetShiftByIdAsync(Guid shiftId, CancellationToken ct = default)
    {
        var shift = await dbContext.WorkShifts.FirstOrDefaultAsync(s => s.Id == shiftId, ct);
        if (shift == null) return null;

        var staff = await dbContext.StaffMembers.FirstOrDefaultAsync(sm => sm.Id == shift.StaffMemberId, ct);

        return new WorkShiftDto(
            shift.Id,
            shift.BranchId,
            shift.StaffMemberId,
            staff?.FullName ?? "Nhân viên",
            shift.OpenedAt,
            shift.ClosedAt,
            shift.OpeningCashFloat,
            shift.SystemCashTotal,
            shift.ActualCashCount,
            shift.VarianceAmount,
            shift.Notes,
            shift.Status);
    }

    public async Task<WorkShiftDto> OpenShiftAsync(OpenShiftRequest request, CancellationToken ct = default)
    {
        var hasOpenShift = await dbContext.WorkShifts.AnyAsync(
            s => s.StaffMemberId == request.StaffMemberId && s.Status == ShiftStatus.Open, ct);

        if (hasOpenShift)
            throw new DomainValidationException("Nhân viên này đang có một ca làm việc chưa đóng.");

        var shift = new WorkShift(request.BranchId, request.StaffMemberId, request.OpeningCashFloat, request.Notes);
        dbContext.WorkShifts.Add(shift);
        await dbContext.SaveChangesAsync(ct);

        return (await GetShiftByIdAsync(shift.Id, ct))!;
    }

    public async Task<ShiftVarianceLogDto> CloseShiftBlindAsync(CloseShiftBlindRequest request, CancellationToken ct = default)
    {
        var shift = await dbContext.WorkShifts.FirstOrDefaultAsync(s => s.Id == request.ShiftId, ct)
            ?? throw new DomainNotFoundException($"Không tìm thấy ca làm việc với Id {request.ShiftId}");

        var systemCash = shift.SystemCashTotal;
        shift.CloseShiftBlind(request.ActualCashCount, systemCash, request.Notes);

        var log = new ShiftClosingVarianceLog(
            shift.Id,
            shift.BranchId,
            shift.OpeningCashFloat,
            systemCash,
            request.ActualCashCount,
            request.Notes);

        dbContext.ShiftClosingVarianceLogs.Add(log);
        await dbContext.SaveChangesAsync(ct);

        return new ShiftVarianceLogDto(
            log.Id,
            log.ShiftId,
            log.OpeningFloat,
            log.SystemRevenue,
            log.ExpectedAmount,
            log.ActualCountedAmount,
            log.VarianceAmount,
            log.CashierNotes,
            log.LoggedAt);
    }

    public async Task<List<ShiftVarianceLogDto>> GetVarianceLogsAsync(Guid branchId, CancellationToken ct = default)
    {
        var logs = await dbContext.ShiftClosingVarianceLogs
            .Where(l => l.BranchId == branchId)
            .OrderByDescending(l => l.LoggedAt)
            .Take(50)
            .ToListAsync(ct);

        return logs.Select(l => new ShiftVarianceLogDto(
            l.Id,
            l.ShiftId,
            l.OpeningFloat,
            l.SystemRevenue,
            l.ExpectedAmount,
            l.ActualCountedAmount,
            l.VarianceAmount,
            l.CashierNotes,
            l.LoggedAt)).ToList();
    }
}
