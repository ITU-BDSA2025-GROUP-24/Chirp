using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using CsvHelper;
using CsvHelper.Configuration;

namespace SimpleDB;

public sealed class CsvDatabase<T> : IDatabaseRepository<T>
{
    private readonly string _filePath;
    private readonly CsvConfiguration _cfg;      // <— _cfg lives here
    private readonly SemaphoreSlim _gate = new(1, 1);

    public CsvDatabase(string filePath)
    {
        _filePath = filePath;

        // Initialize _cfg in the constructor
        _cfg = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            TrimOptions = TrimOptions.Trim,
            DetectDelimiter = true,

            // Be forgiving if the file was created without headers, etc.
            HeaderValidated = null,
            MissingFieldFound = null
        };

        // Optional: create directory if missing (don’t write a file yet)
        var dir = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);
    }

    // Synchronous API to match your existing code/tests
    public IEnumerable<T> Read(int? limit = null)
    {
        _gate.Wait();
        try
        {
            // If file doesn't exist yet, return empty list
            if (!File.Exists(_filePath) || new FileInfo(_filePath).Length == 0)
                return new List<T>();

            using var reader = new StreamReader(_filePath);
            using var csv = new CsvReader(reader, _cfg);

            var records = csv.GetRecords<T>();
            return limit.HasValue ? records.Take(limit.Value).ToList() : records.ToList();
        }
        finally { _gate.Release(); }
    }

    public void Store(T item)
    {
        _gate.Wait();
        try
        {
            var isEmpty = !File.Exists(_filePath) || new FileInfo(_filePath).Length == 0;

            using var stream = new FileStream(_filePath, FileMode.Append, FileAccess.Write, FileShare.Read);
            using var writer = new StreamWriter(stream);
            using var csv = new CsvWriter(writer, _cfg);

            // Ensure header row exists before first record
            if (isEmpty)
            {
                csv.WriteHeader<T>();
                csv.NextRecord();
            }

            csv.WriteRecord(item);
            csv.NextRecord();
        }
        finally { _gate.Release(); }
    }
}
