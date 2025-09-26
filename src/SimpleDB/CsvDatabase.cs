
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System;
using CsvHelper;
using CsvHelper.Configuration;

namespace SimpleDB;

public sealed class CsvDatabase<T> : IDatabaseRepository<T>
{
    private static CsvDatabase<T>? instance;
    private readonly string _filePath;
    private readonly CsvConfiguration _cfg;  
    private readonly SemaphoreSlim _gate = new(1, 1);

    public CsvDatabase(string filePath = "./data/chirp_cli_db.csv")
    {
        _filePath = filePath;

        _cfg = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            TrimOptions = TrimOptions.Trim,
            DetectDelimiter = true,
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
        Console.Write("Searching for " + _filePath);
        _gate.Wait();
        try
        {
            if (!File.Exists(_filePath) || new FileInfo(_filePath).Length == 0)
                return Enumerable.Empty<T>();
                
            using var reader = new StreamReader(_filePath);
            using var csv = new CsvReader(reader, _cfg);

            var records = csv.GetRecords<T>();
            return limit.HasValue ? records.Take(limit.Value).ToList() : records.ToList();
        }
        finally { _gate.Release(); }

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
	