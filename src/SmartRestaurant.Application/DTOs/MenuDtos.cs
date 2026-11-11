namespace SmartRestaurant.Application.DTOs;

using SmartRestaurant.Domain.Enums;

public record MenuCategoryDto(
    Guid Id,
    Guid BranchId,
    string Name,
    string Description,
    int DisplayOrder,
    Guid? ParentCategoryId,
    bool IsActive,
    int ItemCount);

public record MenuItemVariantDto(
    Guid Id,
    Guid MenuItemId,
    string Name,
    decimal PriceAdjustment,
    string Sku,
    bool IsActive);

public record MenuItemDto(
    Guid Id,
    Guid BranchId,
    Guid CategoryId,
    string CategoryName,
    string Code,
    string Name,
    string Description,
    decimal BasePrice,
    string ImageUrl,
    PrepStation PrepStation,
    bool DirectFOHPick,
    bool IsAvailable,
    bool IsCountable,
    int InStockQuantity,
    bool IsActive,
    List<MenuItemVariantDto> Variants);

public record CreateCategoryRequest(
    Guid BranchId,
    string Name,
    string Description = "",
    int DisplayOrder = 0,
    Guid? ParentCategoryId = null);

public record UpdateCategoryRequest(
    Guid CategoryId,
    string Name,
    string Description,
    int DisplayOrder,
    Guid? ParentCategoryId);

public record CreateMenuItemRequest(
    Guid BranchId,
    Guid CategoryId,
    string Code,
    string Name,
    decimal BasePrice,
    string Description = "",
    string ImageUrl = "",
    PrepStation PrepStation = PrepStation.HotKitchen,
    bool DirectFOHPick = false,
    bool IsCountable = true,
    int InStockQuantity = 100);

public record UpdateMenuItemRequest(
    Guid MenuItemId,
    Guid CategoryId,
    string Name,
    decimal BasePrice,
    string Description,
    string ImageUrl,
    PrepStation PrepStation,
    bool DirectFOHPick,
    bool IsCountable);

public record ModifierOptionDto(
    Guid Id,
    Guid ModifierGroupId,
    string Name,
    decimal AdditionalPrice,
    bool IsActive);

public record ModifierGroupDto(
    Guid Id,
    Guid BranchId,
    string Name,
    int MinSelections,
    int MaxSelections,
    bool IsRequired,
    bool IsActive,
    List<ModifierOptionDto> Options);

public record CreateModifierGroupRequest(
    Guid BranchId,
    string Name,
    int MinSelections = 0,
    int MaxSelections = 1);

public record CreateModifierOptionRequest(
    Guid ModifierGroupId,
    string Name,
    decimal AdditionalPrice = 0);

public record CreateVariantRequest(
    Guid MenuItemId,
    string Name,
    decimal PriceAdjustment,
    string Sku);
