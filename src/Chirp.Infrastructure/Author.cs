
namespace Chirp.Infrastructure;

public class Author
{
    //Unique 128 bit ID generated for each author
    public required Guid AuthorId { get; set; }
    public required string Name { get; set; }
    public string? Email { get; set; }
    public List<Cheep> Cheeps { get; set; }
} 
