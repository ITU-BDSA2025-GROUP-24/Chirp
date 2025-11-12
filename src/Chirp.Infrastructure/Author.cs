
namespace Chirp.Infrastructure;

public class Author
{
    public required Guid AuthorId { get; set; }
    public required string Name { get; set; }
    public string? Email { get; set; }
    public ICollection<Cheep> Cheeps { get; set; } = new List<Cheep>();
} 
