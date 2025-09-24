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
    }

    // Synchronous API to match your existing code/tests
    public IEnumerable<T> Read(int? limit = null)
    {
            using var reader = new StreamReader(_filePath);
            using var csv = new CsvReader(reader, _cfg);

            var records = csv.GetRecords<T>();
            return limit.HasValue ? records.Take(limit.Value).ToList() : records.ToList();
    }

    public void Store(T item)
    {
            using var stream = new FileStream(_filePath, FileMode.Append, FileAccess.Write, FileShare.Read);
            using var writer = new StreamWriter(stream);
            using var csv = new CsvWriter(writer, _cfg);
            
            csv.WriteRecord(item);
            csv.NextRecord();
    }
}
