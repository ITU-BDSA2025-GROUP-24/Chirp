namespace Chirp.Core;

public record AuthorInfo(string User, string Email);

public interface IAuthorRepository
{
    Task CreateNewAuthor(string name);
    Task <AuthorDTO> GetAuthorByName (string name);
    
    Task <AuthorDTO> GetAuthorByEmail (string email);
}
