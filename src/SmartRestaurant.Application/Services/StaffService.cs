namespace SmartRestaurant.Application.Services;

using Microsoft.EntityFrameworkCore;
using SmartRestaurant.Application.Common.Interfaces;
using SmartRestaurant.Application.DTOs;
using SmartRestaurant.Application.Interfaces;
using SmartRestaurant.Domain.Entities;
using SmartRestaurant.Domain.Enums;
using SmartRestaurant.Domain.Exceptions;

public class StaffService(IAppDbContext dbContext) : IStaffService
{
    public async Task<List<StaffMemberDto>> GetStaffMembersAsync(Guid branchId, CancellationToken ct = default)
    {
        var staff = await dbContext.StaffMembers
            .Where(s => s.BranchId == branchId && s.IsActive)
            .OrderBy(s => s.Role)
            .ThenBy(s => s.FullName)
            .ToListAsync(ct);

        return staff.Select(MapToDto).ToList();
    }

    public async Task<StaffMemberDto?> GetStaffByIdAsync(Guid staffId, CancellationToken ct = default)
    {
        var member = await dbContext.StaffMembers.FirstOrDefaultAsync(s => s.Id == staffId, ct);
        return member == null ? null : MapToDto(member);
    }

    public async Task<StaffMemberDto> CreateStaffAsync(CreateStaffRequest request, CancellationToken ct = default)
    {
        var exists = await dbContext.StaffMembers.AnyAsync(
            s => s.BranchId == request.BranchId && s.Username == request.Username.Trim().ToLowerInvariant() && s.IsActive, ct);

        if (exists)
            throw new DomainValidationException($"Tên đăng nhập '{request.Username}' đã tồn tại.");

        var member = new StaffMember(
            request.BranchId,
            request.FullName,
            request.Username,
            request.Email,
            request.Role,
            request.PinCode);

        dbContext.StaffMembers.Add(member);
        await dbContext.SaveChangesAsync(ct);

        return MapToDto(member);
    }

    public async Task<StaffMemberDto> UpdateStaffAsync(UpdateStaffRequest request, CancellationToken ct = default)
    {
        var member = await dbContext.StaffMembers.FirstOrDefaultAsync(s => s.Id == request.StaffId, ct)
            ?? throw new DomainNotFoundException($"Không tìm thấy nhân viên với Id {request.StaffId}");

        member.UpdateProfile(request.FullName, request.Email, request.Role);
        await dbContext.SaveChangesAsync(ct);

        return MapToDto(member);
    }

    public async Task<bool> ResetPinAsync(ResetPinRequest request, CancellationToken ct = default)
    {
        var member = await dbContext.StaffMembers.FirstOrDefaultAsync(s => s.Id == request.StaffId, ct);
        if (member == null) return false;

        member.SetPin(request.NewPinCode);
        await dbContext.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> VerifyPinAsync(Guid staffId, string pinCode, CancellationToken ct = default)
    {
        var member = await dbContext.StaffMembers.FirstOrDefaultAsync(s => s.Id == staffId && s.IsActive, ct);
        if (member == null) return false;

        return member.VerifyPin(pinCode);
    }

    public async Task<bool> AuthorizeManagerPinAsync(
        Guid branchId,
        string pinCode,
        string actionName,
        string targetEntity,
        string reason,
        CancellationToken ct = default)
    {
        // Manager or Admin PIN required
        var managers = await dbContext.StaffMembers
            .Where(s => s.BranchId == branchId && s.IsActive &&
                       (s.Role == StaffRole.RestaurantManager || s.Role == StaffRole.SystemAdmin))
            .ToListAsync(ct);

        var authorizedManager = managers.FirstOrDefault(m => m.VerifyPin(pinCode));
        if (authorizedManager == null)
            return false;

        // Log manager PIN audit event per INV-09 / BR-SEC-003
        var auditLog = new ManagerPinAuditLog(
            branchId,
            authorizedManager.Id,
            authorizedManager.FullName,
            actionName,
            targetEntity,
            reason);

        dbContext.ManagerPinAuditLogs.Add(auditLog);
        await dbContext.SaveChangesAsync(ct);

        return true;
    }

    public async Task<bool> ToggleStaffStatusAsync(Guid staffId, CancellationToken ct = default)
    {
        var member = await dbContext.StaffMembers.FirstOrDefaultAsync(s => s.Id == staffId, ct);
        if (member == null) return false;

        if (member.IsActive)
            member.Deactivate();
        else
            member.Activate();

        await dbContext.SaveChangesAsync(ct);
        return true;
    }

    private static StaffMemberDto MapToDto(StaffMember member)
    {
        var roleName = member.Role switch
        {
            StaffRole.SystemAdmin => "Quản Trị Hệ Thống",
            StaffRole.RestaurantManager => "Quản Lý Nhà Hàng",
            StaffRole.Cashier => "Thu Ngân",
            StaffRole.Waiter => "Phục Vụ / Bồi Bàn",
            StaffRole.Chef => "Bếp Trưởng / Đầu Bếp",
            StaffRole.InventoryStaff => "Thủ Kho",
            _ => "Nhân Viên"
        };

        return new StaffMemberDto(
            member.Id,
            member.BranchId,
            member.FullName,
            member.Username,
            member.Email,
            member.Role,
            roleName,
            member.IsActive);
    }
}
