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
    public DbSet<SystemLicense> SystemLicenses => Set<SystemLicense>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderRound> OrderRounds => Set<OrderRound>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<OrderItemModifier> OrderItemModifiers => Set<OrderItemModifier>();
    public DbSet<KitchenTicket> KitchenTickets => Set<KitchenTicket>();
    public DbSet<OrderAuditHistory> OrderAuditHistories => Set<OrderAuditHistory>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<PaymentTransaction> PaymentTransactions => Set<PaymentTransaction>();
    public DbSet<SuspenseLedger> SuspenseLedgers => Set<SuspenseLedger>();

    public Task<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default) =>
        Database.BeginTransactionAsync(cancellationToken);

    public async Task<TResult> ExecuteInTransactionAsync<TResult>(Func<Task<TResult>> operation, CancellationToken cancellationToken = default)
    {
        var strategy = Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await Database.BeginTransactionAsync(cancellationToken);
            var result = await operation();
            await transaction.CommitAsync(cancellationToken);
            return result;
        });
    }

    public async Task ExecuteInTransactionAsync(Func<Task> operation, CancellationToken cancellationToken = default)
    {
        var strategy = Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await Database.BeginTransactionAsync(cancellationToken);
            await operation();
            await transaction.CommitAsync(cancellationToken);
        });
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
