namespace SmartRestaurant.Application.Common.Interfaces;

using Microsoft.EntityFrameworkCore;
using SmartRestaurant.Domain.Entities;

public interface IAppDbContext
{
    DbSet<Branch> Branches { get; }
    DbSet<DiningArea> DiningAreas { get; }
    DbSet<DiningRoom> DiningRooms { get; }
    DbSet<RestaurantTable> RestaurantTables { get; }
    DbSet<MenuCategory> MenuCategories { get; }
    DbSet<MenuItem> MenuItems { get; }
    DbSet<MenuItemVariant> MenuItemVariants { get; }
    DbSet<ModifierGroup> ModifierGroups { get; }
    DbSet<ModifierOption> ModifierOptions { get; }
    DbSet<StaffMember> StaffMembers { get; }
    DbSet<WorkShift> WorkShifts { get; }
    DbSet<ShiftClosingVarianceLog> ShiftClosingVarianceLogs { get; }
    DbSet<ManagerPinAuditLog> ManagerPinAuditLogs { get; }
    DbSet<KitchenWasteLog> KitchenWasteLogs { get; }
    DbSet<DiningSession> DiningSessions { get; }
    DbSet<KitchenStation> KitchenStations { get; }
    DbSet<InventoryItem> InventoryItems { get; }
    DbSet<InventoryTransaction> InventoryTransactions { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
