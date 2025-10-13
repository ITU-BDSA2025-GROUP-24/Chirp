using Microsoft.EntityFrameworkCore;
using Chirp.Core; 

namespace Chirp.Infrastructure;

public class CheepRepository : ICheepRepository
{
    private int pageLength = 32;
    
    private readonly ChirpDBContext _dbContext;

    public CheepRepository(ChirpDBContext dbContext)
    {
        _dbContext = dbContext;
        _dbContext.Database.Migrate();
        DbInitializer.SeedDatabase(_dbContext);
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
            .Skip(pageSize * pageLength)
            .Take(pageLength)
            .Include(c => c.Author)
            .ToListAsync();
       
        var results = new List<CheepDTO>();
        foreach (Cheep cheep in cheeps)
        {
            var result = new CheepDTO
            {
                Author = cheep.Author.Name,
                Cheep = cheep.Text,
                Timestamp = cheep.TimeStamp
            };
            
            results.Add(result); 
        }
        return results;
    }
    
    public async Task<int> CreateCheep(CheepDTO cheep)
    {
        //Does the author exist? No? Create one
        var author = await _dbContext.Authors
            .FirstOrDefaultAsync(a => a.Name == cheep.Author);
    
        if (author == null)
        {
            author = new Author { Name = cheep.Author };
            _dbContext.Authors.Add(author);
        }
        
        //Create new cheep
        Cheep newCheep = new() 
        { 
            Author = author,
            AuthorId = author.AuthorId,
            Text = cheep.Cheep, 
            TimeStamp = cheep.Timestamp
        };
    
        var queryResult = await _dbContext.Cheeps.AddAsync(newCheep);
        await _dbContext.SaveChangesAsync();
        return queryResult.Entity.CheepId;
    }
    
    public async Task UpdateCheep(CheepDTO alteredCheep)
    {
        // Find the existing cheep in the database
        var existingCheep = await _dbContext.Cheeps
            .FirstOrDefaultAsync(c => c.CheepId == alteredCheep.CheepId);
    
        if (existingCheep == null)
        {
            throw new ArgumentException($"Cheep with ID {alteredCheep.CheepId} not found");
        }
    
        existingCheep.Text = alteredCheep.Cheep;
        existingCheep.TimeStamp = alteredCheep.Timestamp;

        await _dbContext.SaveChangesAsync();
    }
    
}
