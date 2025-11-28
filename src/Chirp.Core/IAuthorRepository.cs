namespace Chirp.Core;

public record AuthorInfo(string User, string Email);

public interface IAuthorRepository
{
    Task <Boolean> UserExists(string name, string email);
    Task CreateNewAuthor(string name, string email);
    Task <AuthorDTO> GetAuthorByName (string name);
    Task <AuthorDTO> GetAuthorByEmail (string email);
    Task <AuthorDTO> GetAuthorById(Guid authorId);

    Task<AuthorDTO> AddFollowAsync(String AuthorName, Guid FollowerId);

    Task<Boolean> isFollowing(String AuthorName, Guid FollowerId);

    Task<AuthorDTO> UnFollowAsync(String AuthorName, Guid FollowerId);

    Task<List<Guid>> ReturnFollowing(String AuthorName);
}
