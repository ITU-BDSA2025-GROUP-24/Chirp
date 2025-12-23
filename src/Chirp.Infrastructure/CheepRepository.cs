using Microsoft.EntityFrameworkCore;
using Chirp.Core;

namespace Chirp.Infrastructure;

// Repository class that handles data access operations for Cheep entities
// Implements the ICheepRepository interface
public class CheepRepository : ICheepRepository
{
    //Defines the number of cheeps to display per page to enable pagination
    private readonly int _pageLength = 32;
    
    //Context used to interact with the database
    private readonly ChirpDBContext _dbContext;
   
    /*
    Initializes the repository with database context
    dbContext: Context for accessing the database
    skipMigrations: Flag to skip automatic migrations and seeding  
    */
    public CheepRepository(ChirpDBContext dbContext, bool skipMigrations = false)
    {
        _dbContext = dbContext;
        
        //Run migrations and seed data if skipMigrations is false
        if (!skipMigrations)
        {
            _dbContext.Database.Migrate();
            //Seed database with data from DbInitializer 
            DbInitializer.SeedDatabase(_dbContext);
        }
    }
    
    //Retrieves paginated list of cheeps from the database. List can be filtered by author. 
    public async Task<List<CheepDTO>> ReadCheep(int pageNum = 1, string? author = null)
    {
        int pageSize = pageNum - 1; 
        
        List<Cheep> cheeps = await 
            (
                author == null ? 
                _dbContext.Cheeps :
                _dbContext.Cheeps.Where(c => c.Author.Name == author)
            )
            .OrderByDescending(d => d.TimeStamp)
            .Skip(pageSize * _pageLength)
            .Take(_pageLength)
            .Include(c => c.Author)
            .ToListAsync();
            
       
        var results = new List<CheepDTO>();
        foreach (Cheep cheep in cheeps)
        {
            var result = cheep.ToCheepDTO();
           
            results.Add(result); 
        }
        return results;
    }
    
    public async Task<IEnumerable<CheepDTO>> ReadCheepForAuthors(int pageNum, IEnumerable<Guid> authorIds)
    {
        const int pageSize = 32; 
        var ids = authorIds.ToList();
        if (!ids.Any())
            return Enumerable.Empty<CheepDTO>();

        var query = _dbContext.Cheeps
            .Include(c => c.Author)
            .Where(c => ids.Contains(c.Author.AuthorId))
            .OrderByDescending(c => c.TimeStamp);

        var cheeps = await query
            .Skip((pageNum - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return cheeps.Select(c => c.ToCheepDTO());
    }

    
    public async Task CreateCheep(string username, string email, string cheep)
    {
        Author? author =  await _dbContext.Authors.FirstOrDefaultAsync(a => a.Name == username);
        
        if (author == null)
        {
            throw new UserNotFound("Author name cannot be null or empty");
        }
        
        //Create new cheep
        Cheep newCheep = new Cheep() 
        { 
            Author = author,
            CheepId = Guid.NewGuid(),
            Text = cheep,
            TimeStamp = DateTime.Now
        };

        _dbContext.Cheeps.Add(newCheep);
        await _dbContext.SaveChangesAsync();
    }

    
    public async Task DeleteCheep(Guid cheepId, string username)
    {
        var cheep = await _dbContext.Cheeps
            .Include(c => c.Author)
            .FirstOrDefaultAsync(c => c.CheepId == cheepId);
    
        if (cheep == null)
        {
            throw new InvalidOperationException($"Cheep with ID {cheepId} does not exist.");
        }
        
        //Check if it is the user's cheep
        if (cheep.Author.Name != username)
        {
            throw new InvalidOperationException("You can only delete your own cheeps!");
        }
    
        _dbContext.Cheeps.Remove(cheep);
        await _dbContext.SaveChangesAsync();
    }
}
