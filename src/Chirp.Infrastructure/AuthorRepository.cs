using Chirp.Core;
using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure;

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
    
    public async Task CreateNewAuthor(string name)
    {
        bool userExists = await UserExists(name);
        if (userExists)
        {
            throw new Exception("User already exists.");
        }
        
        //Generates new authorID using Guid
        _dbContext.Authors.Add(new Author {AuthorId = Guid.NewGuid(), Name = name, Cheeps = new List<Cheep>() });
        await _dbContext.SaveChangesAsync();
    }
    
    
   public async Task<AuthorDTO> GetAuthorByName(string name)
    {
        var author = await _dbContext.Authors.FirstOrDefaultAsync(c => c.Name == name);
        
        if (author == null)
        {
            throw new UserNotFound($"The user {name} does not exist.");
        }

        return author.ToAuthorDTO();
    }
    
    public async Task<AuthorDTO> GetAuthorByEmail(string email)
    {
        var mail = await _dbContext.Authors.FirstOrDefaultAsync(c => c.Email == email);
        
        if (mail == null)
        {
            throw new UserNotFound($"The email {email} does not exist.");
        }

        return mail.ToAuthorDTO();
    }

    public async Task<AuthorInfo> GetAuthorInfo(string name, string email)
    {
        var author = await _dbContext.Authors.FirstOrDefaultAsync(c => c.Name == name  && c.Email == email);

        if (author == null)
        {
            throw new UserNotFound($"The user {name} does not exist.");
        }

        var authorInfo = new AuthorInfo(User: author.Name, Email: author.Email);
        
        return authorInfo;
    }
}




