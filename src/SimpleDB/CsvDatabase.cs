using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using SimpleDB;

public sealed class CsvDatabase<T> : IDatabaseRepository<T>
{
    private readonly string _filePath;
    private readonly CsvConfiguration _cfg;
    private static CsvDatabase<T>? instance;

    public CsvDatabase(string filePath = "./data/chirp_cli_db.csv")
    {
        _filePath = filePath;

        // Make sure the directory exists
        var dir = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);

        // CsvHelper config (explicit culture is important)
        _cfg = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true
        };
    }

    public static CsvDatabase<T> getInstance()
    {
        if (instance == null)
        {
            instance = new CsvDatabase<T>();
        }
        return instance;
    }

    public IEnumerable<T> Read(int? limit = null)
    {
        if (!File.Exists(_filePath) || new FileInfo(_filePath).Length == 0)
            return Enumerable.Empty<T>();

        using var reader = new StreamReader(_filePath);
        using var csv = new CsvReader(reader, _cfg);
        var records = csv.GetRecords<T>();
        return limit.HasValue ? records.Take(limit.Value).ToList()
            : records.ToList();
    }

    public void Store(T item)
    {
        var newFile = !File.Exists(_filePath) || new FileInfo(_filePath).Length == 0;

        using var stream = new FileStream(_filePath, FileMode.Append, FileAccess.Write, FileShare.Read);
        using var writer = new StreamWriter(stream);
        using var csv = new CsvWriter(writer, _cfg);

        if (newFile)
        {
            csv.WriteHeader<T>();
            csv.NextRecord();
        }

        csv.WriteRecord(item);
        csv.NextRecord();
    }
}