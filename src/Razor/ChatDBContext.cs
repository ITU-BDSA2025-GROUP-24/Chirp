using Microsoft.EntityFrameworkCore;

namespace MyChat.Razor;

public class ChatDBContext : DbContext
{
    public DbSet<Cheep> Cheep { get; set; }
    public DbSet<Author> Author { get; set; }
    
    public ChatDBContext(DbContextOptions<ChatDBContext> options) : base(options){}
}