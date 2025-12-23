namespace Chirp.Core;

//Interface - Everything below must be implemented in AuthorRepository.
public interface IAuthorRepository
{
    Task <Boolean> UserExists(string name);
    Task CreateNewAuthor(string name, string email);
    Task <AuthorDTO> GetAuthorByName (string name);
    Task <AuthorDTO> GetAuthorByEmail (string email);
    Task <AuthorDTO> GetAuthorById(Guid authorId);

    Task<AuthorDTO> AddFollowAsync(String AuthorName, Guid FollowerId);

    Task<Boolean> isFollowing(String AuthorName, Guid FollowerId);

    Task<AuthorDTO> UnFollowAsync(String AuthorName, Guid FollowerId);

    Task<List<Guid>> ReturnFollowing(String AuthorName);
    Task DeleteAuthor(string name);
    Task UpdateAsync(AuthorDTO author);
}

