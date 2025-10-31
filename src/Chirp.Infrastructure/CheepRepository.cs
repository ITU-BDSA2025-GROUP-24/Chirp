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
            .Reverse()
            .Skip(pageSize * pageLength)
            .Take(pageLength)
            .Include(c => c.Author)
            .ToListAsync();
            
       
        var results = new List<CheepDTO>();
        foreach (Cheep cheep in cheeps)
        {
            var result = new CheepDTO
            {
                Author = new AuthorDTO()
                {
                    Name = cheep.Author.Name,
                    Email = cheep.Author.Email,
                },
                Cheep = cheep.Text,
                TimeStamp = cheep.TimeStamp
            };
            
            results.Add(result); 
        }
        return results;
    }
    
    public async Task CreateCheep(string name, string cheep)
    {
        Author? author = await _dbContext.Authors.Where(a => a.Name == name).FirstOrDefaultAsync();
        
        //var author = new Author { Name = cheep.Author.Name, Email = cheep.Author.Email }; //await _dbContext.Authors.FirstOrDefaultAsync(a => a.Name == cheep.Author.Name);

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new UserNotFound("Author name cannot be null or empty");
        }

        IQueryable<Cheep> Cheeps = _dbContext.Cheeps
            .Where(c => c.Author.Name == name).OrderByDescending(c => c.TimeStamp);
        
        
        
        //Does the author exist? No? Create one
        
        /*if (author == null)
        {
            author = new Author { Name = cheep.Author.Name, Email = $"{cheep.Author.Name}@chirp.com" };
            
            _dbContext.Authors.Add(author);
            await _dbContext.SaveChangesAsync();
        } */
        
        //Create new cheep
        Cheep newCheep = new Cheep() 
        { 
            Author = author,
            CheepId = Guid.NewGuid(),
            Text = cheep,
            TimeStamp = DateTime.Now
        };
    
       /* var queryResult = await _dbContext.Cheeps.AddAsync(newCheep);
        await _dbContext.SaveChangesAsync();
        return queryResult.Entity.CheepId; */
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
        existingCheep.TimeStamp = alteredCheep.TimeStamp;

        await _dbContext.SaveChangesAsync();
    }
    
}
