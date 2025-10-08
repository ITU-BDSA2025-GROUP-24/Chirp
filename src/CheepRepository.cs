namespace MyChat.Razor;

public class CheepRepository : ICheepRepository
{
    public async Task<List<CheepDTO>> ReadCheep(string userName)
    {
        // Formulate the query - will be translated to SQL by EF Core
        var query = _dbContext.Cheep.Select(cheep => new { cheep.Author, cheep.Text });
        // Execute the query
        var result = await query.ToListAsync();

        return result;
        // ...
    }
    
    public async Task<int> CreateCheep(CheepDTO cheep)
    {
        Cheep newCheep = new() { Text = cheep.Text};
        var queryResult = await _dbContext.Messages.AddAsync(newCheep); // does not write to the database!

        await _dbContext.SaveChangesAsync(); // persist the changes in the database
        return queryResult.Entity.CheepId;
    }
    
}
