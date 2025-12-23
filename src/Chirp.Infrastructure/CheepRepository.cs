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
        
        //Query the database for cheeps with author filtering
        List<Cheep> cheeps = await 
            (
                //If there are no author specified, query all cheeps
                //Author specified? Filter cheeps by the specified author's name
                author == null ? 
                _dbContext.Cheeps :
                _dbContext.Cheeps.Where(c => c.Author.Name == author)
            )
            /*
            Sort cheeps by timestamp - Descending order, so new cheeps are shown at the top of the page
            Skip cheeps from previous pages, take cheeps defined by _pageLength, and execute query asynchronously.
            Return as a list.
            */
            .OrderByDescending(d => d.TimeStamp)
            .Skip(pageSize * _pageLength)
            .Take(_pageLength)
            .Include(c => c.Author)
            .ToListAsync();
        
        //Convert the list of cheep entities to CheepDTO objects
        var results = new List<CheepDTO>();
        foreach (Cheep cheep in cheeps)
        {
            //Convert cheep entity to CheepDTO
            var result = cheep.ToCheepDTO();
           
            //Add converted cheep to the result list
            results.Add(result); 
        }
        return results;
    }
    
    //Gets a paginated list of cheeps from the database from specific authors
    public async Task<IEnumerable<CheepDTO>> ReadCheepForAuthors(int pageNum, IEnumerable<Guid> authorIds)
    {
        const int pageSize = 32; 
        
        //Convert authorIDs to a list 
        var ids = authorIds.ToList();
        
        //If no authorIds are provided, return an empty collection
        if (!ids.Any())
            return Enumerable.Empty<CheepDTO>();

        //Build database query used to retrieve cheep from the specified authors, and sort them
        var query = _dbContext.Cheeps
            .Include(c => c.Author)
            .Where(c => ids.Contains(c.Author.AuthorId))
            .OrderByDescending(c => c.TimeStamp);

        //Execute query with pagination
        var cheeps = await query
            .Skip((pageNum - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        //Convert cheep entities to CheepDTo objects
        return cheeps.Select(c => c.ToCheepDTO());
    }

    //Creates a new cheep
    public async Task CreateCheep(string username, string email, string cheep)
    {
        //Find author in the database using the specified username
        Author? author =  await _dbContext.Authors.FirstOrDefaultAsync(a => a.Name == username);
        
        //No author found? Throw exception
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
        //Save changes to the database
        await _dbContext.SaveChangesAsync();
    }

    //Delete cheep from database, after making sure it is the user's own cheep
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
