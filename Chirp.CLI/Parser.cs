using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;

namespace Chirp.CLI;

public static class Parser
{
    public static IEnumerable<Cheep> ComposeCheep()
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

    public static void StoreCheep(String cheeping)
    {
        Cheep newCheep = new Cheep();
        newCheep.Author = Environment.UserName;
        newCheep.Message = cheeping;
        newCheep.Timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
        var records = new List<Cheep>
        {
           newCheep
        };
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            // Don't write the header again.
            HasHeaderRecord = false,
        };
        using (var stream = File.Open("chirp_cli_db.csv", FileMode.Append))
        using (var writer = new StreamWriter(stream))
        using (var csv = new CsvWriter(writer, config))
        {
            csv.WriteRecords(records);
        }
    }
}

//public record Cheep(string Author, string Message, long Timestamp);