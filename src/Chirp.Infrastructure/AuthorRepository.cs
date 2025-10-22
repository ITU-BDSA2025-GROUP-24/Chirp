using Chirp.Core;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure; 

public class AuthorRepository : IAuthorRepository
{
    private ChirpDBContext _dbContext;

    public AuthorRepository(ChirpDBContext _dbContext)
    {
        this._dbContext = _dbContext;
    }

    public async Task<bool> UserExists(string name)
    {
        var author = await _dbContext.Authors.FirstOrDefaultAsync(c => c.Name == name);
        return author != null;
    }
    
    public async Task CreateNewAuthor(int authorId, string name, string email)
    {
        bool userExists = await UserExists(name);
        if (userExists)
        {
            throw new Exception("User already exists.");
        }

        _dbContext.Authors.Add(new Author { Name = name, Cheeps = new List<Cheep>() });
        await _dbContext.SaveChangesAsync();
    }
    
    
   public async Task<AuthorInfo> GetAuthorByName(string name)
    {
        var author = await _dbContext.Authors.FirstOrDefaultAsync(c => c.Name == name);
        
        if (author == null)
        {
            throw new UserNotFound($"The user {name} does not exist.");
        }

        var authorInfo = new AuthorInfo(
            User: author.Name,
            Email: author.Email
        );

        return authorInfo;
    }
    
    public async Task<AuthorInfo> GetAuthorByEmail(string email)
    {
        var mail = await _dbContext.Authors.FirstOrDefaultAsync(c => c.Email == email);
        
        if (mail == null)
        {
            throw new UserNotFound($"The email {email} does not exist.");
        }

        var authorInfo = new AuthorInfo(
            User: mail.Name,
            Email: mail.Email
        );

        return authorInfo;
    }

    public async Task<AuthorInfo> GetAuthorInfo(string name, string email)
    {
        var author = await _dbContext.Authors.FirstOrDefaultAsync(c => c.Name == name  && c.Email == email);

        if (author == null)
        {
            throw new UserNotFound($"The user {name} does not exist.");
        }

        var authorInfo = new AuthorInfo(User: author.Name, 
                                        Email: author.Email
                                        );
        
        return authorInfo;
    }
}




