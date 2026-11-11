namespace SmartRestaurant.Infrastructure.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartRestaurant.Domain.Entities;

public class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Name).HasMaxLength(150).IsRequired();
        builder.Property(b => b.Address).HasMaxLength(300);
        builder.Property(b => b.PhoneNumber).HasMaxLength(50);
        builder.HasQueryFilter(b => !b.IsDeleted);
    }
}

public class DiningAreaConfiguration : IEntityTypeConfiguration<DiningArea>
{
    public void Configure(EntityTypeBuilder<DiningArea> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Name).HasMaxLength(150).IsRequired();
        builder.HasMany(a => a.Rooms).WithOne().HasForeignKey(r => r.DiningAreaId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(a => a.Tables).WithOne().HasForeignKey(t => t.DiningAreaId).OnDelete(DeleteBehavior.Restrict);
        builder.HasQueryFilter(a => !a.IsDeleted);
    }
}

public class DiningRoomConfiguration : IEntityTypeConfiguration<DiningRoom>
{
    public void Configure(EntityTypeBuilder<DiningRoom> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Name).HasMaxLength(150).IsRequired();
        builder.Property(r => r.SurchargeRate).HasPrecision(5, 2);
        builder.Property(r => r.SurchargeFixedAmount).HasPrecision(18, 2);
        builder.HasMany(r => r.Tables).WithOne().HasForeignKey(t => t.DiningRoomId).OnDelete(DeleteBehavior.SetNull);
        builder.HasQueryFilter(r => !r.IsDeleted);
    }
}

public class RestaurantTableConfiguration : IEntityTypeConfiguration<RestaurantTable>
{
    public void Configure(EntityTypeBuilder<RestaurantTable> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Code).HasMaxLength(50).IsRequired();
        builder.Property(t => t.Name).HasMaxLength(150).IsRequired();
        builder.HasIndex(t => new { t.BranchId, t.Code }).IsUnique();
        builder.HasQueryFilter(t => !t.IsDeleted);
    }
}

public class MenuCategoryConfiguration : IEntityTypeConfiguration<MenuCategory>
{
    public void Configure(EntityTypeBuilder<MenuCategory> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).HasMaxLength(150).IsRequired();
        builder.Property(c => c.Description).HasMaxLength(500);
        builder.HasMany(c => c.Items).WithOne().HasForeignKey(i => i.CategoryId).OnDelete(DeleteBehavior.Restrict);
        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}

public class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
{
    public void Configure(EntityTypeBuilder<MenuItem> builder)
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Code).HasMaxLength(50).IsRequired();
        builder.Property(i => i.Name).HasMaxLength(200).IsRequired();
        builder.Property(i => i.Description).HasMaxLength(1000);
        builder.Property(i => i.BasePrice).HasPrecision(18, 2);
        builder.Property(i => i.ImageUrl).HasMaxLength(500);
        builder.HasIndex(i => new { i.BranchId, i.Code }).IsUnique();
        builder.HasMany(i => i.Variants).WithOne().HasForeignKey(v => v.MenuItemId).OnDelete(DeleteBehavior.Cascade);
        builder.HasQueryFilter(i => !i.IsDeleted);
    }
}

public class MenuItemVariantConfiguration : IEntityTypeConfiguration<MenuItemVariant>
{
    public void Configure(EntityTypeBuilder<MenuItemVariant> builder)
    {
        builder.HasKey(v => v.Id);
        builder.Property(v => v.Name).HasMaxLength(100).IsRequired();
        builder.Property(v => v.PriceAdjustment).HasPrecision(18, 2);
        builder.Property(v => v.Sku).HasMaxLength(100);
        builder.HasQueryFilter(v => !v.IsDeleted);
    }
}

public class ModifierGroupConfiguration : IEntityTypeConfiguration<ModifierGroup>
{
    public void Configure(EntityTypeBuilder<ModifierGroup> builder)
    {
        builder.HasKey(g => g.Id);
        builder.Property(g => g.Name).HasMaxLength(150).IsRequired();
        builder.HasMany(g => g.Options).WithOne().HasForeignKey(o => o.ModifierGroupId).OnDelete(DeleteBehavior.Cascade);
        builder.HasQueryFilter(g => !g.IsDeleted);
    }
}

public class ModifierOptionConfiguration : IEntityTypeConfiguration<ModifierOption>
{
    public void Configure(EntityTypeBuilder<ModifierOption> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Name).HasMaxLength(150).IsRequired();
        builder.Property(o => o.AdditionalPrice).HasPrecision(18, 2);
        builder.HasQueryFilter(o => !o.IsDeleted);
    }
}

public class StaffMemberConfiguration : IEntityTypeConfiguration<StaffMember>
{
    public void Configure(EntityTypeBuilder<StaffMember> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.FullName).HasMaxLength(150).IsRequired();
        builder.Property(s => s.Username).HasMaxLength(100).IsRequired();
        builder.Property(s => s.Email).HasMaxLength(150);
        builder.Property(s => s.PinHash).HasMaxLength(128).IsRequired();
        builder.HasIndex(s => new { s.BranchId, s.Username }).IsUnique();
        builder.HasQueryFilter(s => !s.IsDeleted);
    }
}

public class WorkShiftConfiguration : IEntityTypeConfiguration<WorkShift>
{
    public void Configure(EntityTypeBuilder<WorkShift> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.OpeningCashFloat).HasPrecision(18, 2);
        builder.Property(s => s.SystemCashTotal).HasPrecision(18, 2);
        builder.Property(s => s.ActualCashCount).HasPrecision(18, 2);
        builder.Property(s => s.VarianceAmount).HasPrecision(18, 2);
        builder.Property(s => s.Notes).HasMaxLength(500);
        builder.HasQueryFilter(s => !s.IsDeleted);
    }
}

public class ShiftClosingVarianceLogConfiguration : IEntityTypeConfiguration<ShiftClosingVarianceLog>
{
    public void Configure(EntityTypeBuilder<ShiftClosingVarianceLog> builder)
    {
        builder.HasKey(l => l.Id);
        builder.Property(l => l.OpeningFloat).HasPrecision(18, 2);
        builder.Property(l => l.SystemRevenue).HasPrecision(18, 2);
        builder.Property(l => l.ExpectedAmount).HasPrecision(18, 2);
        builder.Property(l => l.ActualCountedAmount).HasPrecision(18, 2);
        builder.Property(l => l.VarianceAmount).HasPrecision(18, 2);
        builder.Property(l => l.CashierNotes).HasMaxLength(500);
        builder.HasQueryFilter(l => !l.IsDeleted);
    }
}

public class ManagerPinAuditLogConfiguration : IEntityTypeConfiguration<ManagerPinAuditLog>
{
    public void Configure(EntityTypeBuilder<ManagerPinAuditLog> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.ManagerName).HasMaxLength(150).IsRequired();
        builder.Property(a => a.ActionName).HasMaxLength(150).IsRequired();
        builder.Property(a => a.TargetEntity).HasMaxLength(200);
        builder.Property(a => a.Reason).HasMaxLength(500);
        builder.HasQueryFilter(a => !a.IsDeleted);
    }
}

public class KitchenWasteLogConfiguration : IEntityTypeConfiguration<KitchenWasteLog>
{
    public void Configure(EntityTypeBuilder<KitchenWasteLog> builder)
    {
        builder.HasKey(w => w.Id);
        builder.Property(w => w.MenuItemName).HasMaxLength(200).IsRequired();
        builder.Property(w => w.Reason).HasMaxLength(500).IsRequired();
        builder.Property(w => w.EstimatedLoss).HasPrecision(18, 2);
        builder.Property(w => w.StaffName).HasMaxLength(150);
        builder.HasQueryFilter(w => !w.IsDeleted);
    }
}

public class DiningSessionConfiguration : IEntityTypeConfiguration<DiningSession>
{
    public void Configure(EntityTypeBuilder<DiningSession> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.SessionCode).HasMaxLength(50).IsRequired();
        builder.Property(s => s.TotalAmount).HasPrecision(18, 2);
        builder.HasQueryFilter(s => !s.IsDeleted);
    }
}

public class KitchenStationConfiguration : IEntityTypeConfiguration<KitchenStation>
{
    public void Configure(EntityTypeBuilder<KitchenStation> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Code).HasMaxLength(50).IsRequired();
        builder.Property(s => s.Name).HasMaxLength(150).IsRequired();
        builder.Property(s => s.Description).HasMaxLength(500);
        builder.Property(s => s.PrinterIpAddress).HasMaxLength(50);
        builder.HasIndex(s => new { s.BranchId, s.Code }).IsUnique();
        builder.HasMany(s => s.MenuItems).WithOne().HasForeignKey(m => m.KitchenStationId).OnDelete(DeleteBehavior.SetNull);
        builder.HasQueryFilter(s => !s.IsDeleted);
    }
}

public class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItem>
{
    public void Configure(EntityTypeBuilder<InventoryItem> builder)
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.ItemCode).HasMaxLength(50).IsRequired();
        builder.Property(i => i.ItemName).HasMaxLength(200).IsRequired();
        builder.Property(i => i.CategoryName).HasMaxLength(100);
        builder.Property(i => i.Unit).HasMaxLength(50);
        builder.HasIndex(i => new { i.BranchId, i.ItemCode }).IsUnique();
        builder.HasQueryFilter(i => !i.IsDeleted);
    }
}

public class InventoryTransactionConfiguration : IEntityTypeConfiguration<InventoryTransaction>
{
    public void Configure(EntityTypeBuilder<InventoryTransaction> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.ItemCode).HasMaxLength(50).IsRequired();
        builder.Property(t => t.ItemName).HasMaxLength(200).IsRequired();
        builder.Property(t => t.Reason).HasMaxLength(500);
        builder.Property(t => t.PerformedBy).HasMaxLength(100);
        builder.HasQueryFilter(t => !t.IsDeleted);
    }
}
