using Microsoft.EntityFrameworkCore;
using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace Chirp.Infrastructure;

public class CheepRepository : ICheepRepository
{
    private int pageLength = 32;
    
    private readonly ChirpDBContext _dbContext;
   
    public CheepRepository(ChirpDBContext dbContext, bool skipMigrations = false)
    {
        _dbContext = dbContext;
        
        if (!skipMigrations)
        {
            _dbContext.Database.Migrate();
            DbInitializer.SeedDatabase(_dbContext);
        }
    }
    
    public async Task<List<CheepDTO>> ReadCheep(int pageNum = 1, string? author = null)
    {
        int pageSize = pageNum - 1; 
        
        List<Cheep> cheeps = await 
            (
                author == null ? 
                _dbContext.Cheeps :
                _dbContext.Cheeps.Where(c => c.Author.Name == author)
            )
            .OrderBy(c => c.TimeStamp)
            .Reverse()
            .Skip(pageSize * pageLength)
            .Take(pageLength)
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
    
    public async Task<int> CreateCheep(CheepDTO cheep)
    {
        Author author = new Author { Name = cheep.Author.Name, Email = cheep.Author.Email, Cheeps = new List<Cheep>() };
 
        if (string.IsNullOrWhiteSpace(cheep.Author.Name))
        {
            throw new UserNotFound("Author name cannot be null or empty");
        }
        
       //Create new cheep
        Cheep newCheep = new Cheep() 
        { 
            Author = author,
            Text = cheep.Cheep,
            TimeStamp = DateTime.Now
        };
    
        var result = await _dbContext.Cheeps.AddAsync(newCheep);
        await _dbContext.SaveChangesAsync();
        return result.Entity.CheepId;

    }
    
}
