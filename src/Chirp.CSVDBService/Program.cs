using System.Text.Json;
using Chirp.CSVDBService;
using SimpleDB;

class Program
{
    public record Cheep(string Author, string Message, long Timestamp);

    static IDatabaseRepository<Cheep> dbRepository;
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

// register your repo; point to your CSV path
        builder.Services.AddSingleton<IDatabaseRepository<Cheep>>(sp =>
        {
            // Replace 'CsvDatabase<Cheep>' with your actual implementation class if different
            return new CsvDatabase<Cheep>();
        });

        var app = builder.Build();

        app.UseSwagger();
        app.UseSwaggerUI();

        app.MapGet("/cheeps", (IDatabaseRepository<Cheep> repo, int? limit) =>
        {
            var items = repo.Read(limit);
            return Results.Ok(items);
        });

        app.MapPost("/cheep", (IDatabaseRepository<Cheep> repo, Cheep dto) =>
        {
            dbRepository.Store(dto);
            return Results.Created($"/cheeps", dto);
        });

        app.Run();
    }

    private static string GetCheeps(int cap = 10)
    {
        IEnumerable<Cheep> records = dbRepository.Read(cap);
        return JsonSerializer.Serialize(records);
    }

    private static IResult StoreCheep(Cheep newCheep)
    {
        dbRepository.Store(newCheep);
        return Results.Created($"/cheep/{newCheep.Timestamp}", newCheep);
    }
}
/*using Microsoft.AspNetCore.Http.Json;
using SimpleDB;
using Chirp.CSVDBService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JsonOptions>(o =>
{
    o.SerializerOptions.PropertyNamingPolicy = null; 
});


var dataPath = Environment.GetEnvironmentVariable("CHEEP_DB_PATH");
if (string.IsNullOrWhiteSpace(dataPath))
{
    var root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
    dataPath = Path.Combine(root, "data", "cheeps.csv");
}

builder.Services.AddSingleton<IDatabaseRepository<Cheep>>(_ =>
    new CsvDatabase<Cheep>(dataPath));

var app = builder.Build();

app.MapGet("/", () => Results.Ok(new { ok = true, service = "Chirp CSV DB" }));

app.MapGet("/cheeps", (IDatabaseRepository<Cheep> repo) => Results.Ok(repo.Read()));
app.MapPost("/cheep", (Cheep c, IDatabaseRepository<Cheep> repo) => { repo.Store(c); return Results.Ok(new { stored = true }); });

app.Run();

public partial class Program { }
*/
