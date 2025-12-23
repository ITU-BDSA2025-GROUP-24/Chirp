namespace Chirp.Core;

//Interface - Everything below must be implemented in CheepRepository
public interface ICheepRepository
{
    public Task CreateCheep(string username, string email, string cheep);

    public Task<List<CheepDTO>> ReadCheep(int pageNum = 1, string? author = null);
    
    Task<IEnumerable<CheepDTO>> ReadCheepForAuthors(int pageNum, IEnumerable<Guid> authorIds);
    
    public Task DeleteCheep(Guid cheepId, string username);
}
