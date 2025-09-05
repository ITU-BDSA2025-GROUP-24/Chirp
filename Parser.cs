using System.Globalization;
using CsvHelper;

namespace Chirp.CLI;

public class Parser
{
    public static IEnumerable<Cheep> ComposeCheep(string[] args)
    {
        using (var reader = new StreamReader("chirp_cli_db.csv"))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            csv.Context.RegisterClassMap<CheepMap>();
            var records = csv.GetRecords<Cheep>();
            return records;
        }
    }
}

//public record Cheep(string Author, string Message, long Timestamp);