namespace SmartRestaurant.Application.Services;

using Microsoft.EntityFrameworkCore;
using SmartRestaurant.Application.Common.Interfaces;
using SmartRestaurant.Application.DTOs;
using SmartRestaurant.Application.Interfaces;
using SmartRestaurant.Domain.Entities;
using SmartRestaurant.Domain.Exceptions;

public class MenuService(IAppDbContext dbContext) : IMenuService
{
    public async Task<List<MenuCategoryDto>> GetCategoriesAsync(Guid branchId, CancellationToken ct = default)
    {
        var categories = await dbContext.MenuCategories
            .Where(c => c.BranchId == branchId && c.IsActive)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync(ct);

        var result = new List<MenuCategoryDto>();
        foreach (var category in categories)
        {
            var itemCount = await dbContext.MenuItems
                .CountAsync(i => i.CategoryId == category.Id && i.IsActive, ct);

            result.Add(new MenuCategoryDto(
                category.Id,
                category.BranchId,
                category.Name,
                category.Description,
                category.DisplayOrder,
                category.ParentCategoryId,
                category.IsActive,
                itemCount));
        }

        return result;
    }

    public async Task<MenuCategoryDto?> GetCategoryByIdAsync(Guid categoryId, CancellationToken ct = default)
    {
        var category = await dbContext.MenuCategories.FirstOrDefaultAsync(c => c.Id == categoryId, ct);
        if (category == null) return null;

        var itemCount = await dbContext.MenuItems
            .CountAsync(i => i.CategoryId == category.Id && i.IsActive, ct);

        return new MenuCategoryDto(
            category.Id,
            category.BranchId,
            category.Name,
            category.Description,
            category.DisplayOrder,
            category.ParentCategoryId,
            category.IsActive,
            itemCount);
    }

    public async Task<MenuCategoryDto> CreateCategoryAsync(CreateCategoryRequest request, CancellationToken ct = default)
    {
        var category = new MenuCategory(request.BranchId, request.Name, request.Description, request.DisplayOrder, request.ParentCategoryId);
        dbContext.MenuCategories.Add(category);
        await dbContext.SaveChangesAsync(ct);

        return new MenuCategoryDto(
            category.Id,
            category.BranchId,
            category.Name,
            category.Description,
            category.DisplayOrder,
            category.ParentCategoryId,
            category.IsActive,
            0);
    }

    public async Task<MenuCategoryDto> UpdateCategoryAsync(UpdateCategoryRequest request, CancellationToken ct = default)
    {
        var category = await dbContext.MenuCategories.FirstOrDefaultAsync(c => c.Id == request.CategoryId, ct)
            ?? throw new DomainNotFoundException($"Không tìm thấy danh mục với Id {request.CategoryId}");

        category.UpdateDetails(request.Name, request.Description, request.DisplayOrder, request.ParentCategoryId);
        await dbContext.SaveChangesAsync(ct);

        return (await GetCategoryByIdAsync(category.Id, ct))!;
    }

    public async Task<bool> DeleteCategoryAsync(Guid categoryId, CancellationToken ct = default)
    {
        var category = await dbContext.MenuCategories.FirstOrDefaultAsync(c => c.Id == categoryId, ct);
        if (category == null) return false;

        category.Deactivate();
        await dbContext.SaveChangesAsync(ct);
        return true;
    }

    public async Task<List<MenuItemDto>> GetMenuItemsAsync(Guid branchId, Guid? categoryId = null, string? search = null, CancellationToken ct = default)
    {
        var query = dbContext.MenuItems.Where(i => i.BranchId == branchId && i.IsActive);

        if (categoryId.HasValue && categoryId.Value != Guid.Empty)
            query = query.Where(i => i.CategoryId == categoryId.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLowerInvariant();
            query = query.Where(i => i.Name.ToLower().Contains(s) || i.Code.ToLower().Contains(s));
        }

        var items = await query.OrderBy(i => i.Code).ToListAsync(ct);
        var categories = await dbContext.MenuCategories.Where(c => c.BranchId == branchId).ToDictionaryAsync(c => c.Id, c => c.Name, ct);

        var result = new List<MenuItemDto>();
        foreach (var item in items)
        {
            var variants = await dbContext.MenuItemVariants
                .Where(v => v.MenuItemId == item.Id && v.IsActive)
                .ToListAsync(ct);

            var categoryName = categories.TryGetValue(item.CategoryId, out var name) ? name : "Chung";

            result.Add(new MenuItemDto(
                item.Id,
                item.BranchId,
                item.CategoryId,
                categoryName,
                item.Code,
                item.Name,
                item.Description,
                item.BasePrice,
                item.ImageUrl,
                item.PrepStation,
                item.DirectFOHPick,
                item.IsAvailable,
                item.IsCountable,
                item.InStockQuantity,
                item.IsActive,
                variants.Select(v => new MenuItemVariantDto(v.Id, v.MenuItemId, v.Name, v.PriceAdjustment, v.Sku, v.IsActive)).ToList()));
        }

        return result;
    }

    public async Task<MenuItemDto?> GetMenuItemByIdAsync(Guid menuItemId, CancellationToken ct = default)
    {
        var item = await dbContext.MenuItems.FirstOrDefaultAsync(i => i.Id == menuItemId, ct);
        if (item == null) return null;

        var category = await dbContext.MenuCategories.FirstOrDefaultAsync(c => c.Id == item.CategoryId, ct);
        var variants = await dbContext.MenuItemVariants
            .Where(v => v.MenuItemId == item.Id && v.IsActive)
            .ToListAsync(ct);

        return new MenuItemDto(
            item.Id,
            item.BranchId,
            item.CategoryId,
            category?.Name ?? "Chung",
            item.Code,
            item.Name,
            item.Description,
            item.BasePrice,
            item.ImageUrl,
            item.PrepStation,
            item.DirectFOHPick,
            item.IsAvailable,
            item.IsCountable,
            item.InStockQuantity,
            item.IsActive,
            variants.Select(v => new MenuItemVariantDto(v.Id, v.MenuItemId, v.Name, v.PriceAdjustment, v.Sku, v.IsActive)).ToList());
    }

    public async Task<MenuItemDto> CreateMenuItemAsync(CreateMenuItemRequest request, CancellationToken ct = default)
    {
        var exists = await dbContext.MenuItems.AnyAsync(
            i => i.BranchId == request.BranchId && i.Code == request.Code.Trim().ToUpperInvariant() && i.IsActive, ct);

        if (exists)
            throw new DomainValidationException($"Mã món ăn '{request.Code}' đã tồn tại trong hệ thống.");

        var item = new MenuItem(
            request.BranchId,
            request.CategoryId,
            request.Code,
            request.Name,
            request.BasePrice,
            request.Description,
            request.ImageUrl,
            request.PrepStation,
            request.DirectFOHPick,
            request.IsCountable,
            request.InStockQuantity);

        dbContext.MenuItems.Add(item);
        await dbContext.SaveChangesAsync(ct);

        return (await GetMenuItemByIdAsync(item.Id, ct))!;
    }

    public async Task<MenuItemDto> UpdateMenuItemAsync(UpdateMenuItemRequest request, CancellationToken ct = default)
    {
        var item = await dbContext.MenuItems.FirstOrDefaultAsync(i => i.Id == request.MenuItemId, ct)
            ?? throw new DomainNotFoundException($"Không tìm thấy món ăn với Id {request.MenuItemId}");

        item.UpdateDetails(
            request.CategoryId,
            request.Name,
            request.BasePrice,
            request.Description,
            request.ImageUrl,
            request.PrepStation,
            request.DirectFOHPick,
            request.IsCountable);

        await dbContext.SaveChangesAsync(ct);

        return (await GetMenuItemByIdAsync(item.Id, ct))!;
    }

    public async Task<bool> ToggleAvailabilityAsync(Guid menuItemId, CancellationToken ct = default)
    {
        var item = await dbContext.MenuItems.FirstOrDefaultAsync(i => i.Id == menuItemId, ct);
        if (item == null) return false;

        item.ToggleAvailability();
        await dbContext.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> UpdateStockAsync(Guid menuItemId, int newQuantity, CancellationToken ct = default)
    {
        var item = await dbContext.MenuItems.FirstOrDefaultAsync(i => i.Id == menuItemId, ct);
        if (item == null) return false;

        item.SetStockQuantity(newQuantity);
        await dbContext.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteMenuItemAsync(Guid menuItemId, CancellationToken ct = default)
    {
        var item = await dbContext.MenuItems.FirstOrDefaultAsync(i => i.Id == menuItemId, ct);
        if (item == null) return false;

        item.Deactivate();
        await dbContext.SaveChangesAsync(ct);
        return true;
    }

    public async Task<List<ModifierGroupDto>> GetModifierGroupsAsync(Guid branchId, CancellationToken ct = default)
    {
        var groups = await dbContext.ModifierGroups
            .Where(g => g.BranchId == branchId && g.IsActive)
            .OrderBy(g => g.Name)
            .Include(g => g.Options)
            .ToListAsync(ct);

        return groups.Select(g => new ModifierGroupDto(
            g.Id,
            g.BranchId,
            g.Name,
            g.MinSelections,
            g.MaxSelections,
            g.IsRequired,
            g.IsActive,
            g.Options.Where(o => o.IsActive).Select(o => new ModifierOptionDto(
                o.Id,
                o.ModifierGroupId,
                o.Name,
                o.AdditionalPrice,
                o.IsActive
            )).ToList()
        )).ToList();
    }

    public async Task<ModifierGroupDto> CreateModifierGroupAsync(CreateModifierGroupRequest request, CancellationToken ct = default)
    {
        var group = new ModifierGroup(request.BranchId, request.Name, request.MinSelections, request.MaxSelections);
        dbContext.ModifierGroups.Add(group);
        await dbContext.SaveChangesAsync(ct);

        return new ModifierGroupDto(
            group.Id,
            group.BranchId,
            group.Name,
            group.MinSelections,
            group.MaxSelections,
            group.IsRequired,
            group.IsActive,
            []);
    }

    public async Task<ModifierOptionDto> AddModifierOptionAsync(CreateModifierOptionRequest request, CancellationToken ct = default)
    {
        var group = await dbContext.ModifierGroups.FirstOrDefaultAsync(g => g.Id == request.ModifierGroupId, ct)
            ?? throw new DomainNotFoundException($"Không tìm thấy nhóm tùy chọn với ID: {request.ModifierGroupId}");

        var option = new ModifierOption(request.ModifierGroupId, request.Name, request.AdditionalPrice);
        dbContext.ModifierOptions.Add(option);
        await dbContext.SaveChangesAsync(ct);

        return new ModifierOptionDto(option.Id, option.ModifierGroupId, option.Name, option.AdditionalPrice, option.IsActive);
    }

    public async Task<bool> DeleteModifierGroupAsync(Guid groupId, CancellationToken ct = default)
    {
        var group = await dbContext.ModifierGroups.FirstOrDefaultAsync(g => g.Id == groupId, ct);
        if (group == null) return false;

        group.Deactivate();
        await dbContext.SaveChangesAsync(ct);
        return true;
    }

    public async Task<MenuItemVariantDto> AddVariantAsync(CreateVariantRequest request, CancellationToken ct = default)
    {
        var item = await dbContext.MenuItems.FirstOrDefaultAsync(i => i.Id == request.MenuItemId, ct)
            ?? throw new DomainNotFoundException($"Không tìm thấy món ăn với ID: {request.MenuItemId}");

        var variant = new MenuItemVariant(request.MenuItemId, request.Name, request.PriceAdjustment, request.Sku);
        dbContext.MenuItemVariants.Add(variant);
        await dbContext.SaveChangesAsync(ct);

        return new MenuItemVariantDto(variant.Id, variant.MenuItemId, variant.Name, variant.PriceAdjustment, variant.Sku, variant.IsActive);
    }

    public async Task<bool> DeleteVariantAsync(Guid variantId, CancellationToken ct = default)
    {
        var variant = await dbContext.MenuItemVariants.FirstOrDefaultAsync(v => v.Id == variantId, ct);
        if (variant == null) return false;

        variant.Deactivate();
        await dbContext.SaveChangesAsync(ct);
        return true;
    }
}
