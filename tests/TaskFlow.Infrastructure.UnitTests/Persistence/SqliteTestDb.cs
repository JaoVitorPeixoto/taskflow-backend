using System;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Infrastructure.Persistence;

namespace TaskFlow.Infrastructure.UnitTests.Persistence;

public sealed class SqliteTestDb : IAsyncDisposable
{
    private readonly SqliteConnection _connection;
    public AppDbContext Context { get; private set; }

    public SqliteTestDb()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;
        
        Context = new AppDbContext(options);
        Context.Database.EnsureCreated();
    }

    public async ValueTask DisposeAsync()
    {
        await Context.DisposeAsync();
        await _connection.DisposeAsync();
    }
}
