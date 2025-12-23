using Chirp.Core;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure; 

public class AuthorRepository(ChirpDBContext dbContext) : IAuthorRepository
{
    //Method used to check whether a user with the given name exists.
    public async Task<bool> UserExists(string name)
    {
        var author = await dbContext.Authors.FirstOrDefaultAsync(c => c.Name == name);
        return author != null;
    }
    
    /*
    Creates new author. First it checks whether the author already exists. 
    If so, it throws an exception to avoid duplicates.
    */
    public async Task CreateNewAuthor(string name, string email)
    {
        //Does the user exist? Throw exception.
        if (await UserExists(name))
        {
            throw new InvalidOperationException("User already exists.");
        }
        
        //Otherwise create a new author, and save the changes to the database
        dbContext.Authors.Add(new Author {Name = name, Email = email, AuthorId = Guid.NewGuid()});
        await dbContext.SaveChangesAsync();
    }
    
    /*
     Gets an author by username. Throws exception if author not found. 
     Returns an AuthorDTO.
    */
   public async Task<AuthorDTO> GetAuthorByName(string name)
    {
        var author = await dbContext.Authors.FirstOrDefaultAsync(c => c.Name == name);
        
        if (author == null)
        {
            throw new UserNotFound($"The user {name} does not exist.");
        }
        //Converts the author entity to an AuthorDTO and returns it
        return author.ToAuthorDTO();
    }
    
   /*
    Gets author by email. Throws an UserNotFound exception if author is not found.
    Returns an AuthorDTO. 
   */
    public async Task<AuthorDTO> GetAuthorByEmail(string email)
    {
        var mail = await dbContext.Authors.FirstOrDefaultAsync(c => c.Email == email);
        
        if (mail == null)
        {
            throw new UserNotFound($"The email {email} does not exist.");
        }
        //Converts the email entity to an AuthorDTO and returns it
        return mail.ToAuthorDTO();
    }
    
    /*
    Gets author by their AuthorId. Throws an UserNotFound exception if author is not found.
    Returns an AuthorDTO. 
    */
    public async Task<AuthorDTO> GetAuthorById(Guid id)
    {
        var authorId = await dbContext.Authors.FirstOrDefaultAsync(c => c.AuthorId == id);

        if (authorId == null)
        {
            throw new UserNotFound($"The author {authorId} does not exist.");
        }
        //Converts authorId entity to an AuthorDTO and returns it
        return authorId.ToAuthorDTO(); 

    }
    
    //Implements follow functionality. 
    public async Task<AuthorDTO> AddFollowAsync(string authorName, Guid followId)
    {
        //Query database and find author by name
        var author = await dbContext.Authors.FirstOrDefaultAsync(a => a.Name == authorName);
        
        //Throw exception if the author does not exist
        if (author == null)
            throw new UserNotFound($"The user {authorName} does not exist.");
        
        //Check whether the author already follows the user to avoid duplicates
        if (!author.FollowsId.Contains(followId))
        {
            //Add new follow ID to the author's follows collection
            author.FollowsId.Add(followId);
            await dbContext.SaveChangesAsync();
        }
        //Converts author entity to an AuthorDTO and returns it
        return author.ToAuthorDTO();
    }

    //Checks whether an author is following a user
    public async Task<bool> isFollowing(string authorName, Guid followId)
    {
        var author = await dbContext.Authors.FirstOrDefaultAsync(a => a.Name == authorName);
        if (author == null)
        {
            throw new UserNotFound($"The user {authorName} does not exist.");
        }
        //Return true if the author follows the user
        return author.FollowsId.Contains(followId);
    }

    //Returns a list of users followed by the author.
    public async Task<List<Guid>> ReturnFollowing(string authorName)
    {
        var author = await dbContext.Authors.FirstOrDefaultAsync(a => a.Name == authorName);
        if (author == null)
        {
            throw new UserNotFound($"The user {authorName} does not exist.");
        }
        //Converts the author's follows collection to a list and returns it 
        return author.FollowsId.ToList();
    }

    //Implements unfollow functionality.     
    public async Task<AuthorDTO> UnFollowAsync(string authorName, Guid followId)
    {
        //Query database, and find author by name
        var author = await dbContext.Authors.FirstOrDefaultAsync(a => a.Name == authorName);
        
        //Throw exception if author does not exist
        if (author == null)
        {
            throw new UserNotFound($"The user {authorName} does not exist.");
        }
        
        //Checks whether the author's follows collection contains the id of the user they want to unfollow
        if (author.FollowsId.Contains(followId))
        {
            //Remove the follow ID from the author's follow collection
            author.FollowsId.Remove(followId);
            //Save changes to database
            await dbContext.SaveChangesAsync();
        }
        //Converts the author entity to an AuthorDTO and returns it
        return author.ToAuthorDTO();
    }

    //Deletes an author and all information about them from the database (Forget Me functionality) 
    public async Task DeleteAuthor(string name)

    {
        //Query database and find author and their cheeps 
        var author = await dbContext.Authors.Include(a => a.Cheeps).FirstOrDefaultAsync(a => a.Name == name);
        
        //Throw exception if the user does not exist in the database
        if (author == null)
        {
            throw new UserNotFound($"The user {name} does not exist.");
        }
        
        //Store the author's ID for lookup 
        var authorId = author.AuthorId;
        
        //Find all users who are following the author 
        var followingThisAuthor = await dbContext.Authors.Where(a => a.FollowsId.Contains(authorId)).ToListAsync();
        //Remove the author from follower's follow collection 
        foreach (var follows in followingThisAuthor)
        {
            follows.FollowsId.Remove(authorId);
        }
        //Delete all cheeps created by the author from the database
        dbContext.Cheeps.RemoveRange(author.Cheeps);
        //Delete the author from the database
        dbContext.Authors.Remove(author);
        //Save changes to the database 
        await dbContext.SaveChangesAsync();
        
    }
    
    //Updates an existing author's information in the database, used to update profile picture
    public async Task UpdateAsync(AuthorDTO authorDto)
    {
        //Query database, find author by ID 
        var author = await dbContext.Authors
            .FirstOrDefaultAsync(a => a.AuthorId == authorDto.AuthorId);

        if (author is null)
        {
            throw new InvalidOperationException($"Author with id '{authorDto.AuthorId}' not found.");
        }
        
        //Update the author's profile image URL
        author.ProfileImageUrl = authorDto.ProfileImageUrl;
        
        //Save changes to the database
        await dbContext.SaveChangesAsync();
    }
}




