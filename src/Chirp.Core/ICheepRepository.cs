namespace Chirp.Core;

public interface ICheepRepository
{
    public Task CreateCheep(string name, string cheep);

    public Task<List<CheepDTO>> ReadCheep(int pageNum = 1, string? author = null);

    public Task UpdateCheep(CheepDTO alteredCheep);
}
