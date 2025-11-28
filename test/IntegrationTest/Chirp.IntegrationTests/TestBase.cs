using Chirp.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Chirp.IntegrationTests;

public abstract class TestBase
{
    protected DbContextOptions<ChirpDBContext> CreateInMemoryOptions()
    {
        var conn = new SqliteConnection("Filename=:memory:");
        conn.Open();

        return new DbContextOptionsBuilder<ChirpDBContext>()
            .UseSqlite(conn)
            .Options;
    }
}