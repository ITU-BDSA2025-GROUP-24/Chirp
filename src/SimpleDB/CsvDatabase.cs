using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using CsvHelper;
using CsvHelper.Configuration;

namespace SimpleDB;

public sealed class CsvDatabase<T> : IDatabaseRepository<T>
{
    private readonly string filePath;
    private readonly CsvConfiguration _config;

    public CsvDatabase(string? customFilePath = null, bool hasHeader = true)
    {
        this.filePath = customFilePath ?? "chirp_cli_db.csv";

        _config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = hasHeader,
            HeaderValidated = null,
            MissingFieldFound = null,
            BadDataFound = null
        };
    }
		
		   //Checking path for .csv
		 /*  
		 Console.WriteLine($"Current working directory: {Directory.GetCurrentDirectory()}");
		Console.WriteLine($"Looking for file at relative path: {filePath}");
		Console.WriteLine($"Full resolved path: {Path.GetFullPath(filePath)}");
		Console.WriteLine($"File exists at that path: {File.Exists(filePath)}");
		Console.WriteLine();
		*/



	public IEnumerable<T> Read(int? limit = null)
	{
		//If .csv does not exist then return empty collection to avoid crashing
		if (!File.Exists(filePath))
		{
			return Enumerable.Empty<T>();
		}
		
		List<T>? rec;
	    using (var reader = new StreamReader(filePath))
		using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture)) //From https://joshclose.github.io/CsvHelper/
		{
			if(_config.HasHeaderRecord && csv.Read())
			{ 
				csv.ReadHeader();
			}

			rec = csv.GetRecords<T>().ToList();
		}
	    
	    //Return specific amount of Cheeps if the limit is lower than the amount of registered Cheeps
		if (limit != null && limit < rec.Count)
		{
			int n = limit.GetValueOrDefault();
			return rec.GetRange(rec.Count - n, n); //Return the last N records
		}

		return rec;
	}

	public void Store(T record)
	{
		var fileExists = File.Exists(filePath);
		//Append - Add the new cheep to the end of file
		using (var writer = new StreamWriter(filePath, true))	
		using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture)) //From https://joshclose.github.io/CsvHelper/
		{
			if (!fileExists && _config.HasHeaderRecord)
            {
                csv.WriteHeader<T>();
                csv.NextRecord();
            }
			
			csv.WriteRecord(record); //writes to csv
			csv.NextRecord();
		}
	}
}