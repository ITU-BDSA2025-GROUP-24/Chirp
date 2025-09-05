using CsvHelper.Configuration;

namespace Chirp.CLI;

public class CheepMap : ClassMap<Cheep> {
    public CheepMap()
    {
        Map<object>(m => m.Author).Name("Author");
        Map<object>(m => m.Message).Name("Message");
        Map<object>(m => m.Timestamp).Name("Timestamp");
    }
}
    
