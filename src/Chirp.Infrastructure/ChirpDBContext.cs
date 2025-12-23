using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure;


// Database context for managing Cheeps and Authors
public class ChirpDBContext : DbContext
{
    public DbSet<Cheep> Cheeps { get; set; }
    public DbSet<Author> Authors { get; set; }
 
    public ChirpDBContext(DbContextOptions<ChirpDBContext> options) : base(options) {}
   
}
