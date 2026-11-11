namespace SmartRestaurant.Application.Interfaces;

using SmartRestaurant.Application.DTOs;

public interface ISettingService
{
    Task<RestaurantSettingDto> GetSettingsAsync(Guid branchId, CancellationToken cancellationToken = default);
    Task<RestaurantSettingDto> UpdateCutoffAsync(UpdateBusinessDayCutoffRequest request, CancellationToken cancellationToken = default);
    Task<RestaurantSettingDto> UpdateVipSurchargesAsync(UpdateVipSurchargeRequest request, CancellationToken cancellationToken = default);
    Task<RestaurantSettingDto> ConfigureModulesAsync(UpdateModuleConfigRequest request, CancellationToken cancellationToken = default);
    Task<RestaurantSettingDto> ApplyPresetAsync(ApplyPresetRequest request, CancellationToken cancellationToken = default);
}
