using Microsoft.EntityFrameworkCore;

namespace MyChat.Razor;

public class ChatDBContext : DbContext
{
    public DbSet<Message> Messages { get; set; }
    public DbSet<User> Users { get; set; }
    
    public ChatDBContext(DbContextOptions<ChatDBContext> options) : base(options){}
}