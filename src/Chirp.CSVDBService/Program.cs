using Microsoft.AspNetCore.Http.Json;
using SimpleDB;
using Chirp.CSVDBService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JsonOptions>(o =>
{
    o.SerializerOptions.PropertyNamingPolicy = null; // keep Author/Message/Timestamp
});


// decide CSV path
var dataPath = Environment.GetEnvironmentVariable("CHEEP_DB_PATH");
if (string.IsNullOrWhiteSpace(dataPath))
{
    // solution root/data/cheeps.csv relative to build output
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
