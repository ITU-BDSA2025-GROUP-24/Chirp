using Chirp.Core;

public class AuthorRepository : IAuthorRepository
{
    private ChirpDBContext _dbContext;

    public AuthorRepository(ChirpDBContext _dbContext)
    {
        this._dbContext = _dbContext;
    }

    public async Task<bool> UserExists(string name)
    {
        var author = await _dbcontext.Authors.FirstOrDefaultAsync(c => c.Name == name);
        return author != null;
    }
    
    public async void CreateNewAuthor(int authorID, string name, string email)
    {
        bool userExists = await UserExists(name);
        if (userExists)
        {
            throw new Exception("User already exists.");
        }
    }

    public async Task<AuthorInfo> GetAuthorInfo(string name)
    {
        var author = await _dbcontext.Authors.FirstOrDefaultAsync(c => c.Name == name);

        if (author == null)
        {
            throw new UserNotFoundException($"The user {name} does not exist.");
        }

        var authorInfo = new AuthorInfo(User: author.Name, Email: author.Email);
        
        return authorInfo;
    }
    
}




