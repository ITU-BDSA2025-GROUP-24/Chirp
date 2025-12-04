namespace Chirp.Core;

public class AuthorDTO
{
    public required string Name { get; set; }
    
    public required Guid AuthorId { get; set; }
    public required string Email { get; set; }
    
    public List<Guid> FollowsId { get; set; } =  new List<Guid>();
    
    public string? ProfileImageUrl { get; set; }
}
