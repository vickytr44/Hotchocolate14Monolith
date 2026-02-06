using HotChocolateV14.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace HotChocolateV14.Test;

public sealed class DbContextFixture : IDisposable
{
    public ApplicationContext DbContext { get; }

    private readonly SqliteConnection _connection;

    public DbContextFixture()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<ApplicationContext>()
            .UseSqlite(_connection)
            .Options;

        DbContext = new ApplicationContext(options);
        DbContext.Database.EnsureCreated();

        // Optional seed
        DbContext.Customers.Add(new Customer { Id = 1, Name = "Alice" });
        DbContext.SaveChanges();
    }

    public void Dispose()
    {
        DbContext.Dispose();
        _connection.Dispose(); // IMPORTANT for SQLite in-memory
    }
}
