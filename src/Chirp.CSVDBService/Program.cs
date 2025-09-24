using System.Globalization;
using System.Text.Json;
using Microsoft.AspNetCore.Http.Json;
using CsvHelper;
using CsvHelper.Configuration;

public class Program {
    public record Cheep(string Author, string Message, long Timestamp);
    private readonly string datapath;
    public static void Main(string[] args) {
        
        CsvConfiguration _cfg;    
        _cfg = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            TrimOptions = TrimOptions.Trim,
            DetectDelimiter = true,
            
            HeaderValidated = null,
            MissingFieldFound = null
        };
        
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

        var app = builder.Build();

        app.MapGet("/", () => Results.Ok(new { ok = true, service = "Chirp CSV DB" }));

        app.MapGet("/cheeps", (int length) => Read<Cheep>(10, _cfg, dataPath));
        app.MapPost("/cheep", (HttpRequest request) => Store(request, _cfg, dataPath));

        app.Run();

    }
    
    
    public static IEnumerable<T> Read<T>(int length, CsvConfiguration _cfg, string datapath)
    {
    
        using var reader = new StreamReader(datapath);
        using var csv = new CsvReader(reader, _cfg);

        var records = csv.GetRecords<T>().ToList();
        return records;
    }

    public static void Store(HttpRequest request,CsvConfiguration _cfg, string datapath)
    {
        using var stream = new FileStream(datapath, FileMode.Append, FileAccess.Write, FileShare.Read);
        using var writer = new StreamWriter(stream);
        using var csv = new CsvWriter(writer, _cfg);
        var item = JsonSerializer.Serialize(request);
            
        csv.WriteRecord(item);
        csv.NextRecord();
    }
}
