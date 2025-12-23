namespace Chirp.Core;


public class AuthorDTO
{
    //Name, AuthorId, Email - Basic author information 
    public required string Name { get; set; }
    
    public required Guid AuthorId { get; set; }
    public required string Email { get; set; }
    
    //Optional - FollowsId, used to implement follow/unfollow function. List of authors the user follow
    public List<Guid> FollowsId { get; set; } =  new List<Guid>();
    
    //Optional (not required) profile picture URL 
    public string? ProfileImageUrl { get; set; }
}
