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

// ---- Choose a persistent, writable path on Azure ----
        var home = Environment.GetEnvironmentVariable("HOME");     // "/home" on Linux App Service
        var contentRoot = builder.Environment.ContentRootPath;      // local dev fallback
        var dataDir = string.IsNullOrEmpty(home)
            ? Path.Combine(contentRoot, "data")
            : Path.Combine(home, "data");                           // => /home/data (persists)
        Directory.CreateDirectory(dataDir);

        var dataPath = Path.Combine(dataDir, "chirp_cli_db.csv");
        
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();   // <-- required

// Optional: let Azure App Settings override this
        var configuredPath = builder.Configuration["CHEEP_DB_PATH"];
        if (!string.IsNullOrWhiteSpace(configuredPath))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(configuredPath)!);
            dataPath = configuredPath;
        }

// ---- Register your repo with the explicit path ----
        builder.Services.AddSingleton<IDatabaseRepository<Cheep>>(_ => new CsvDatabase<Cheep>(dataPath));

        var app = builder.Build();

// ---- Debug endpoint to see exactly where the file is ----
        app.MapGet("/_diag/path", () =>
        {
            return Results.Ok(new
            {
                HOME = home,
                ContentRoot = contentRoot,
                DataDir = dataDir,
                DataPath = dataPath,
                Exists = System.IO.File.Exists(dataPath),
                Length = System.IO.File.Exists(dataPath) ? new FileInfo(dataPath).Length : 0
            });
        });


        app.UseSwagger();
        app.UseSwaggerUI();

        app.MapGet("/cheeps", (IDatabaseRepository<Cheep> repo, int? limit) =>
        {
            var items = repo.Read(limit);
            return Results.Ok(items);
        });

        app.MapPost("/cheep", (IDatabaseRepository<Cheep> repo, Cheep dto, ILogger<Program> log) =>
        {
            try
            {
                log.LogInformation("Storing cheep to {Path} | Author={Author}", dataPath, dto.Author);
                repo.Store(dto);
                return Results.Created("/cheeps", dto);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Failed writing cheep to {Path}", dataPath);
                return Results.Problem("Failed to store cheep. Check logs for details.");
            }
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
