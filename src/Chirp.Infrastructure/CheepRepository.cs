using Microsoft.EntityFrameworkCore;
using Chirp.Core;
using Chirp.Infrastructure;

namespace Chirp.Infrastructure;

public class CheepRepository : ICheepRepository
{
    private int pageLength = 32;
    
    private readonly ChirpDBContext _dbContext;
    private readonly IAuthorRepository _authorRepo; 

    public CheepRepository(ChirpDBContext dbContext, IAuthorRepository authorRepo, bool skipMigrations = false)
    {
        _dbContext = dbContext;
        _authorRepo = authorRepo;
        
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
    
    public async Task CreateCheep(string name, string email, string cheep)
    {
        
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new UserNotFound("Author name cannot be null or empty");
        }
        
       //Does the author exist? No? Create one
        if (await _authorRepo.UserExists(name))
        {
           await _authorRepo.CreateNewAuthor(name, email); 
        }
        
        Author? author = await _dbContext.Authors.Where(a => a.Name == name).FirstOrDefaultAsync();
      
        //Create new cheep
        Cheep newCheep = new Cheep() 
        { 
            Author = author,
            Text = cheep,
            TimeStamp = DateTime.Now
        };
    
        _dbContext.Cheeps.Add(newCheep);
        await _dbContext.SaveChangesAsync();
       
          
        // IQueryable<Cheep> Cheeps = _dbContext.Cheeps
        //.Where(c => c.Author.Name == name).OrderByDescending(c => c.TimeStamp);
        
        /* var queryResult = await _dbContext.Cheeps.AddAsync(newCheep);
        
        return queryResult.Entity.CheepId; */
    }
    
}
