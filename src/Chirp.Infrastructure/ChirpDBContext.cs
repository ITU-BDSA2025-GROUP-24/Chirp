using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure
{

    public class ChirpDBContext : DbContext
    {
        public DbSet<Cheep> Cheeps { get; set; }
        public DbSet<Author> Authors { get; set; }
        public string DbPath { get; }

        public ChirpDBContext(DbContextOptions<ChirpDBContext> options) : base(options)
        {
            var path = Path.GetTempPath();
            DbPath = Path.Join(path, "chirp.db");
            Console.WriteLine(DbPath);
        }

        // Configures EF to create a Sqlite database file in the special "local" folder
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");
    }
}
