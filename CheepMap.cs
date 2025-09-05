using CsvHelper.Configuration;

namespace Chirp.CLI;

public sealed class CheepMap : ClassMap<Cheep>
{
    public CheepMap()
    {
        Map(m => m.Author).Name("Author");
        Map(m => m.Message).Name("Message");      // CsvHelper handles quotes/commas/newlines
        Map(m => m.Timestamp).Name("Timestamp");
    }
}
    
