namespace SmartRestaurant.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;
using SmartRestaurant.Application.Common.Interfaces;
using SmartRestaurant.Domain.Entities;
using SmartRestaurant.Infrastructure.Data.Configurations;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IAppDbContext
{
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<DiningArea> DiningAreas => Set<DiningArea>();
    public DbSet<DiningRoom> DiningRooms => Set<DiningRoom>();
    public DbSet<RestaurantTable> RestaurantTables => Set<RestaurantTable>();
    public DbSet<MenuCategory> MenuCategories => Set<MenuCategory>();
    public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    public DbSet<MenuItemVariant> MenuItemVariants => Set<MenuItemVariant>();
    public DbSet<ModifierGroup> ModifierGroups => Set<ModifierGroup>();
    public DbSet<ModifierOption> ModifierOptions => Set<ModifierOption>();
    public DbSet<StaffMember> StaffMembers => Set<StaffMember>();
    public DbSet<WorkShift> WorkShifts => Set<WorkShift>();
    public DbSet<ShiftClosingVarianceLog> ShiftClosingVarianceLogs => Set<ShiftClosingVarianceLog>();
    public DbSet<ManagerPinAuditLog> ManagerPinAuditLogs => Set<ManagerPinAuditLog>();
    public DbSet<KitchenWasteLog> KitchenWasteLogs => Set<KitchenWasteLog>();
    public DbSet<DiningSession> DiningSessions => Set<DiningSession>();
    public DbSet<KitchenStation> KitchenStations => Set<KitchenStation>();
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
    public DbSet<InventoryTransaction> InventoryTransactions => Set<InventoryTransaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
