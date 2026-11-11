namespace SmartRestaurant.Application.Tests;

using Microsoft.EntityFrameworkCore;
using SmartRestaurant.Infrastructure.Data;

public static class TestDbContextFactory
{
    public static AppDbContext Create(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        return new AppDbContext(options);
    }
}
