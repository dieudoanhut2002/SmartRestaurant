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

public class SystemLicenseConfiguration : IEntityTypeConfiguration<SystemLicense>
{
    public void Configure(EntityTypeBuilder<SystemLicense> builder)
    {
        builder.HasKey(l => l.Id);
        builder.Property(l => l.LicenseId).HasMaxLength(100).IsRequired();
        builder.Property(l => l.CustomerId).HasMaxLength(200).IsRequired();
        builder.Property(l => l.InstallationId).HasMaxLength(100).IsRequired();
        builder.Property(l => l.ProductCode).HasMaxLength(50).IsRequired();
        builder.Property(l => l.Edition).HasMaxLength(50).IsRequired();
        builder.Property(l => l.FeaturesJson).IsRequired();
        builder.Property(l => l.DigitalSignature).IsRequired();
        builder.Property(l => l.RawDocument).IsRequired();
        builder.OwnsOne(l => l.GracePolicy);
        builder.HasIndex(l => l.IsActive);
        builder.HasIndex(l => l.LicenseId);
        builder.HasQueryFilter(l => !l.IsDeleted);
    }
}

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(o => o.OrderCode).HasMaxLength(50).IsRequired();
        builder.Property(o => o.Subtotal).HasPrecision(18, 2);
        builder.Property(o => o.Surcharges).HasPrecision(18, 2);
        builder.Property(o => o.DiscountAmount).HasPrecision(18, 2);
        builder.Property(o => o.TotalAmount).HasPrecision(18, 2);
        builder.HasMany(o => o.Rounds).WithOne().HasForeignKey(r => r.OrderId).OnDelete(DeleteBehavior.Cascade);
        builder.HasQueryFilter(o => !o.IsDeleted);
    }
}

public class OrderRoundConfiguration : IEntityTypeConfiguration<OrderRound>
{
    public void Configure(EntityTypeBuilder<OrderRound> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.IdempotencyKey).HasMaxLength(100).IsRequired();
        builder.Property(r => r.Note).HasMaxLength(500);
        builder.HasIndex(r => r.IdempotencyKey).IsUnique();
        builder.HasMany(r => r.Items).WithOne().HasForeignKey(i => i.OrderRoundId).OnDelete(DeleteBehavior.Cascade);
        builder.HasQueryFilter(r => !r.IsDeleted);
    }
}

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.ItemName).HasMaxLength(200).IsRequired();
        builder.Property(i => i.UnitPrice).HasPrecision(18, 2);
        builder.Property(i => i.KitchenNote).HasMaxLength(500);
        builder.HasMany(i => i.Modifiers).WithOne().HasForeignKey(m => m.OrderItemId).OnDelete(DeleteBehavior.Cascade);
        builder.HasQueryFilter(i => !i.IsDeleted);
    }
}

public class OrderItemModifierConfiguration : IEntityTypeConfiguration<OrderItemModifier>
{
    public void Configure(EntityTypeBuilder<OrderItemModifier> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.OptionName).HasMaxLength(100).IsRequired();
        builder.Property(m => m.PriceAdjustment).HasPrecision(18, 2);
        builder.HasQueryFilter(m => !m.IsDeleted);
    }
}

public class KitchenTicketConfiguration : IEntityTypeConfiguration<KitchenTicket>
{
    public void Configure(EntityTypeBuilder<KitchenTicket> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.TicketNumber).HasMaxLength(50).IsRequired();
        builder.Property(t => t.Notes).HasMaxLength(500);
        builder.Property(t => t.ItemsJson).IsRequired();
        builder.HasQueryFilter(t => !t.IsDeleted);
    }
}

public class OrderAuditHistoryConfiguration : IEntityTypeConfiguration<OrderAuditHistory>
{
    public void Configure(EntityTypeBuilder<OrderAuditHistory> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Action).HasMaxLength(100).IsRequired();
        builder.Property(a => a.Details).HasMaxLength(1000);
        builder.HasQueryFilter(a => !a.IsDeleted);
    }
}

public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.EventType).HasMaxLength(100).IsRequired();
        builder.Property(m => m.PayloadJson).IsRequired();
        builder.HasIndex(m => m.ProcessedAt);
        builder.HasQueryFilter(m => !m.IsDeleted);
    }
}

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.InvoiceNumber).HasMaxLength(100).IsRequired();
        builder.Property(i => i.CustomerName).HasMaxLength(150);
        builder.Property(i => i.CustomerPhone).HasMaxLength(50);
        builder.Property(i => i.Subtotal).HasPrecision(18, 2);
        builder.Property(i => i.SurchargeAmount).HasPrecision(18, 2);
        builder.Property(i => i.DiscountAmount).HasPrecision(18, 2);
        builder.Property(i => i.TaxableAmount).HasPrecision(18, 2);
        builder.Property(i => i.VatRate).HasPrecision(5, 2);
        builder.Property(i => i.VatAmount).HasPrecision(18, 2);
        builder.Property(i => i.TotalAmount).HasPrecision(18, 2);
        builder.Property(i => i.PaidAmount).HasPrecision(18, 2);
        builder.Property(i => i.ChangeAmount).HasPrecision(18, 2);
        builder.Property(i => i.Notes).HasMaxLength(500);
        builder.Property(i => i.ItemsJson).IsRequired();
        builder.HasMany(i => i.Transactions).WithOne().HasForeignKey(t => t.InvoiceId).OnDelete(DeleteBehavior.Cascade);
        builder.HasQueryFilter(i => !i.IsDeleted);
    }
}

public class PaymentTransactionConfiguration : IEntityTypeConfiguration<PaymentTransaction>
{
    public void Configure(EntityTypeBuilder<PaymentTransaction> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Amount).HasPrecision(18, 2);
        builder.Property(t => t.TransactionRef).HasMaxLength(100);
        builder.HasQueryFilter(t => !t.IsDeleted);
    }
}

public class SuspenseLedgerConfiguration : IEntityTypeConfiguration<SuspenseLedger>
{
    public void Configure(EntityTypeBuilder<SuspenseLedger> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Amount).HasPrecision(18, 2);
        builder.Property(s => s.TransactionRef).HasMaxLength(100).IsRequired();
        builder.Property(s => s.BankCode).HasMaxLength(50);
        builder.Property(s => s.PayerInfo).HasMaxLength(200);
        builder.Property(s => s.Reason).HasMaxLength(300).IsRequired();
        builder.Property(s => s.ResolutionNotes).HasMaxLength(500);
        builder.HasQueryFilter(s => !s.IsDeleted);
    }
}
