
namespace Chirp.Infrastructure;

public class Author
{
    public int AuthorId { get; set; }
    public required string Name { get; set; }
    public string? Email { get; set; }
    public ICollection<Cheep> Cheeps { get; set; }
} 
