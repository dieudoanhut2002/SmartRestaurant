namespace SmartRestaurant.Application.Services;

using Microsoft.EntityFrameworkCore;
using SmartRestaurant.Application.Common.Interfaces;
using SmartRestaurant.Application.DTOs;
using SmartRestaurant.Application.Interfaces;
using SmartRestaurant.Domain.Entities;
using SmartRestaurant.Domain.Exceptions;

public class SettingService(IAppDbContext dbContext) : ISettingService
{
    public async Task<RestaurantSettingDto> GetSettingsAsync(Guid branchId, CancellationToken cancellationToken = default)
    {
        var branch = await GetBranchAsync(branchId, cancellationToken);
        return MapToDto(branch);
    }

    public async Task<RestaurantSettingDto> UpdateCutoffAsync(UpdateBusinessDayCutoffRequest request, CancellationToken cancellationToken = default)
    {
        var branch = await GetBranchAsync(request.BranchId, cancellationToken);
        branch.UpdateBusinessDayCutoff(request.CutoffTime);
        await dbContext.SaveChangesAsync(cancellationToken);
        return MapToDto(branch);
    }

    public async Task<RestaurantSettingDto> UpdateVipSurchargesAsync(UpdateVipSurchargeRequest request, CancellationToken cancellationToken = default)
    {
        var branch = await GetBranchAsync(request.BranchId, cancellationToken);
        branch.UpdateVipSurcharges(request.SurchargeRate, request.SurchargeFixedAmount);
        await dbContext.SaveChangesAsync(cancellationToken);
        return MapToDto(branch);
    }

    public async Task<RestaurantSettingDto> ConfigureModulesAsync(UpdateModuleConfigRequest request, CancellationToken cancellationToken = default)
    {
        var branch = await GetBranchAsync(request.BranchId, cancellationToken);
        branch.ConfigureModules(
            request.IsTableReservationEnabled,
            request.IsVietQrEnabled,
            request.IsInventoryEnabled,
            request.IsReportingEnabled);
        await dbContext.SaveChangesAsync(cancellationToken);
        return MapToDto(branch);
    }

    public async Task<RestaurantSettingDto> ApplyPresetAsync(ApplyPresetRequest request, CancellationToken cancellationToken = default)
    {
        var branch = await GetBranchAsync(request.BranchId, cancellationToken);
        branch.ApplyOnboardingPreset(request.PresetName);
        await dbContext.SaveChangesAsync(cancellationToken);
        return MapToDto(branch);
    }

    private async Task<Branch> GetBranchAsync(Guid branchId, CancellationToken cancellationToken)
    {
        Branch? branch = null;
        if (branchId != Guid.Empty)
        {
            branch = await dbContext.Branches.FirstOrDefaultAsync(b => b.Id == branchId, cancellationToken);
        }

        branch ??= await dbContext.Branches.FirstOrDefaultAsync(cancellationToken);

        return branch ?? throw new DomainNotFoundException("Không tìm thấy thông tin chi nhánh nhà hàng.");
    }

    private static RestaurantSettingDto MapToDto(Branch b) => new(
        b.Id,
        b.Name,
        b.Address,
        b.PhoneNumber,
        b.BusinessDateCutoffTime,
        $"{b.BusinessDateCutoffTime:hh\\:mm}",
        b.VipRoomDefaultSurchargeRate,
        b.VipRoomDefaultFixedAmount,
        b.IsTableReservationEnabled,
        b.IsVietQrEnabled,
        b.IsInventoryEnabled,
        b.IsReportingEnabled
    );
}
