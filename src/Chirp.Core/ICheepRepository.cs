namespace Chirp.Core;

public interface ICheepRepository
{
    public Task<int> CreateCheep(CheepDTO newCheep);

    public Task<List<CheepDTO>> ReadCheep(int pageNum = 1, string? author = null);

    public Task UpdateCheep(CheepDTO alteredCheep);
}
