using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;

namespace Chirp.CLI;

public static class Parser
{
    public static IEnumerable<Cheep> ComposeCheep(string[] args)
    {
        var cfg = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            TrimOptions = TrimOptions.Trim,                 // trims all field values
            PrepareHeaderForMatch = h => h.Header.Trim(),   // trims header names like " Message"
            IgnoreBlankLines = true,
            DetectColumnCountChanges = true
        };

        using var reader = new StreamReader("chirp_cli_db.csv", Encoding.UTF8);
        using var csv = new CsvReader(reader, cfg);
        csv.Context.RegisterClassMap<CheepMap>();

        return csv.GetRecords<Cheep>().ToList();           // materialize before disposing
    }
}

//public record Cheep(string Author, string Message, long Timestamp);