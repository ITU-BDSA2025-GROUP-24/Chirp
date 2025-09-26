using System.Net.Http.Json;

namespace Chirp.CLI;

public sealed class ChirpApi
{
    private readonly HttpClient _http;
    public ChirpApi(HttpClient http) => _http = http;

    public async Task<IReadOnlyList<Cheep>> GetAllAsync()
        => (await _http.GetFromJsonAsync<List<Cheep>>("/cheeps")) ?? new();

    public async Task<bool> PostAsync(Cheep c)
    {
        var res = await _http.PostAsJsonAsync("/cheep", c);
        return res.IsSuccessStatusCode;
    }
}
