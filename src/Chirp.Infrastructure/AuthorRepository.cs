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

    public async Task<bool> UserExists(string name, string email)
    {
        var author = await _dbContext.Authors.FirstOrDefaultAsync(c => c.Name == name);
        return author != null;
    }
    
    public async Task CreateNewAuthor(string name, string email)
    {
        bool userExists = await UserExists(name, email);
        if (userExists)
        {
            throw new Exception("User already exists.");
        }
        
        _dbContext.Authors.Add(new Author {Name = name, Email = email, Cheeps = new List<Cheep>(), AuthorId = Guid.NewGuid()});
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
}




