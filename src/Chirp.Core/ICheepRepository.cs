namespace Chirp.Core;

public interface ICheepRepository
{
    public Task CreateCheep(string name, string email, string cheep);

    public Task<List<CheepDTO>> ReadCheep(int pageNum = 1, string? author = null);
}
