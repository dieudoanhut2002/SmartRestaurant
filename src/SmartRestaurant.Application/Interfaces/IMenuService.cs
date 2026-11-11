namespace SmartRestaurant.Application.Interfaces;

using SmartRestaurant.Application.DTOs;

public interface IMenuService
{
    Task<List<MenuCategoryDto>> GetCategoriesAsync(Guid branchId, CancellationToken ct = default);
    Task<MenuCategoryDto?> GetCategoryByIdAsync(Guid categoryId, CancellationToken ct = default);
    Task<MenuCategoryDto> CreateCategoryAsync(CreateCategoryRequest request, CancellationToken ct = default);
    Task<MenuCategoryDto> UpdateCategoryAsync(UpdateCategoryRequest request, CancellationToken ct = default);
    Task<bool> DeleteCategoryAsync(Guid categoryId, CancellationToken ct = default);

    Task<List<MenuItemDto>> GetMenuItemsAsync(Guid branchId, Guid? categoryId = null, string? search = null, CancellationToken ct = default);
    Task<MenuItemDto?> GetMenuItemByIdAsync(Guid menuItemId, CancellationToken ct = default);
    Task<MenuItemDto> CreateMenuItemAsync(CreateMenuItemRequest request, CancellationToken ct = default);
    Task<MenuItemDto> UpdateMenuItemAsync(UpdateMenuItemRequest request, CancellationToken ct = default);
    Task<bool> ToggleAvailabilityAsync(Guid menuItemId, CancellationToken ct = default);
    Task<bool> UpdateStockAsync(Guid menuItemId, int newQuantity, CancellationToken ct = default);
    Task<bool> DeleteMenuItemAsync(Guid menuItemId, CancellationToken ct = default);

    Task<List<ModifierGroupDto>> GetModifierGroupsAsync(Guid branchId, CancellationToken ct = default);
    Task<ModifierGroupDto> CreateModifierGroupAsync(CreateModifierGroupRequest request, CancellationToken ct = default);
    Task<ModifierOptionDto> AddModifierOptionAsync(CreateModifierOptionRequest request, CancellationToken ct = default);
    Task<bool> DeleteModifierGroupAsync(Guid groupId, CancellationToken ct = default);
    Task<MenuItemVariantDto> AddVariantAsync(CreateVariantRequest request, CancellationToken ct = default);
    Task<bool> DeleteVariantAsync(Guid variantId, CancellationToken ct = default);
}
