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
        var app = builder.Build();

        // simple home page
        app.MapGet("/", () => Results.Text("Chirp.CSVDBService is running", "text/plain"));

        // GET /cheeps?cap=10  (cap is optional; defaults to 10)
        app.MapGet("/cheeps", (int cap = 10) =>
        {
            var records = GetCheeps(cap);
            return Results.Json(records);
        });

        // POST /cheep  with JSON body { "author": "...", "message": "...", "timestamp": 1234567890 }
        // Minimal API binds the JSON body to Cheep automatically
        app.MapPost("/cheep", (Cheep newCheep) =>
        {
            StoreCheep(newCheep);
            return Results.Created($"/cheep/{newCheep.Timestamp}", newCheep);
        });

        app.Run();
    }

    private static string GetCheeps(int cap = 10)
    {
        IEnumerable<Cheep> records = dbRepository.Read(cap);
        return JsonSerializer.Serialize(records);
    }

    private static async void StoreCheep(HttpRequest request)
    {
        var body = new StreamReader(request.Body);
        string jsonObject = await body.ReadToEndAsync();
        Console.WriteLine(jsonObject);
        Cheep newCheep = JsonSerializer.Deserialize<Cheep>(jsonObject);
        Console.WriteLine(newCheep.Author);
        Console.WriteLine(newCheep.Timestamp);
        Console.WriteLine(newCheep.Message);
        dbRepository.Store(newCheep);
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
