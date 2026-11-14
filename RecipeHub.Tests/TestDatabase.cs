using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using RecipeHub.Infrastructure;

namespace RecipeHub.Tests;

// SQLite ověřuje i relační omezení; každá zkouška má vlastní prázdnou databázi.
internal sealed class TestDatabase : IDisposable
{
    private readonly SqliteConnection _connection = new("Data Source=:memory:");
    public AppDbContext Context { get; }

    public TestDatabase()
    {
        _connection.Open();
        Context = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options);
        Context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        Context.Dispose();
        _connection.Dispose();
    }
}
