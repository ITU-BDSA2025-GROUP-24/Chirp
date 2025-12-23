using Chirp.Core;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure; 

public class AuthorRepository(ChirpDBContext dbContext) : IAuthorRepository
{
    public async Task<bool> UserExists(string name)
    {
        var author = await dbContext.Authors.FirstOrDefaultAsync(c => c.Name == name);
        return author != null;
    }
    
    public async Task CreateNewAuthor(string name, string email)
    {
        bool userExists = await UserExists(name);
        if (userExists)
        {
            throw new Exception("User already exists.");
        }
        
        dbContext.Authors.Add(new Author {Name = name, Email = email, Cheeps = new List<Cheep>(), AuthorId = Guid.NewGuid()});
        await dbContext.SaveChangesAsync();
    }
    
    
   public async Task<AuthorDTO> GetAuthorByName(string name)
    {
        var author = await dbContext.Authors.FirstOrDefaultAsync(c => c.Name == name);
        
        if (author == null)
        {
            throw new UserNotFound($"The user {name} does not exist.");
        }

        return author.ToAuthorDTO();
    }
    
    public async Task<AuthorDTO> GetAuthorByEmail(string email)
    {
        var mail = await dbContext.Authors.FirstOrDefaultAsync(c => c.Email == email);
        
        if (mail == null)
        {
            throw new UserNotFound($"The email {email} does not exist.");
        }

        return mail.ToAuthorDTO();
    }

    public async Task<AuthorDTO> GetAuthorById(Guid id)
    {
        var authorId = await dbContext.Authors.FirstOrDefaultAsync(c => c.AuthorId == id);

        if (authorId == null)
        {
            throw new UserNotFound($"The author {authorId} does not exist.");
        }

        return authorId.ToAuthorDTO(); 

    }
    
    public async Task<AuthorDTO> AddFollowAsync(string authorName, Guid followId)
    {
        var author = await dbContext.Authors.FirstOrDefaultAsync(a => a.Name == authorName);
        if (author == null)
            throw new UserNotFound($"The user {authorName} does not exist.");
        
        if (!author.FollowsId.Contains(followId))
        {
            author.FollowsId.Add(followId);
            await dbContext.SaveChangesAsync();
        }
        
        return author.ToAuthorDTO();
    }

    public async Task<bool> isFollowing(string authorName, Guid followId)
    {
        var author = await dbContext.Authors.FirstOrDefaultAsync(a => a.Name == authorName);
        if (author == null)
        {
            throw new UserNotFound($"The user {authorName} does not exist.");
        }
        return author.FollowsId.Contains(followId);
    }

    public async Task<List<Guid>> ReturnFollowing(string authorName)
    {
        var author = await dbContext.Authors.FirstOrDefaultAsync(a => a.Name == authorName);
        if (author == null)
        {
            throw new UserNotFound($"The user {authorName} does not exist.");
        }

        return author.FollowsId.ToList();
    }

    public async Task<AuthorDTO> UnFollowAsync(string authorName, Guid followId)
    {
        var author = await dbContext.Authors.FirstOrDefaultAsync(a => a.Name == authorName);
        if (author == null)
        {
            throw new UserNotFound($"The user {authorName} does not exist.");
        }

        if (author.FollowsId.Contains(followId))
        {
            author.FollowsId.Remove(followId);
            await dbContext.SaveChangesAsync();
        }

        return author.ToAuthorDTO();
    }

    public async Task DeleteAuthor(string name)

    {
        var author = await dbContext.Authors.Include(a => a.Cheeps).FirstOrDefaultAsync(a => a.Name == name);
        
        if (author == null)
        {
            throw new UserNotFound($"The user {name} does not exist.");
        }
        
        var authorId = author.AuthorId;
        
        var followingThisAuthor = await dbContext.Authors.Where(a => a.FollowsId.Contains(authorId)).ToListAsync();

        foreach (var follows in followingThisAuthor)
        {
            follows.FollowsId.Remove(authorId);
        }
        
        dbContext.Cheeps.RemoveRange(author.Cheeps);
        
        dbContext.Authors.Remove(author);
        
        await dbContext.SaveChangesAsync();
        
    }
    public async Task UpdateAsync(AuthorDTO authorDto)
    {
        var author = await dbContext.Authors
            .FirstOrDefaultAsync(a => a.AuthorId == authorDto.AuthorId);

        if (author is null)
        {
            throw new InvalidOperationException($"Author with id '{authorDto.AuthorId}' not found.");
        }

        author.ProfileImageUrl = authorDto.ProfileImageUrl;

        await dbContext.SaveChangesAsync();
    }
}




