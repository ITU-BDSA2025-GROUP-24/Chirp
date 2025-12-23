
namespace Chirp.Infrastructure;


//Database entity that maps to our database table. 
public class Author
{
    /*
    Guid used instead of int. Makes it harder to guess authorId since it is not generated as an int
    that is incremented each time an author is created.
    */
    public required Guid AuthorId { get; set; }
    public required string Name { get; set; }
    public string? Email { get; set; }
    public ICollection<Cheep> Cheeps { get; set; } = new List<Cheep>();
    public List<Guid> FollowsId { get; set; } =  new List<Guid>();
    public string? ProfileImageUrl { get; set; }
} 
