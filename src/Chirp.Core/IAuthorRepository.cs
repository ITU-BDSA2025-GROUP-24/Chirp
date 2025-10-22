namespace Chirp.Core;

public record AuthorInfo(string User, string Email);

public interface IAuthorRepository
{
    Task CreateNewAuthor(int authorId, string name, string email);
    Task<AuthorInfo> GetAuthorByName (string name);
    
    Task<AuthorInfo> GetAuthorByEmail (string email);
}
