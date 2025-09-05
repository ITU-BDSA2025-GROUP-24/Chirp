
using System.Globalization; 
using System.Reflection; 
using System.Reflection.Metadata;
using CsvHelper; 


namespace SimpleDB;

public class CSVDatabase<T> : IDatabaseRepository<T>
{
	private string filePath; 
	
	public CSVDatabase(string filePath = "../../chirp_cli_db.csv")
	{
		this.filePath = filePath; 
		
		foreach(Property in prop in typeof(T).GetProperties())
		{
			
		}	
    }
	
	public IEnumerable<T> Read(int? limit = null)
	{
		List<T>? record = null; 
		using (var reader = new StreamReader(filePath))
		using (var csv = new CsvReader(reader,
			       CultureInfo.InvariantCulture)) //From https://joshclose.github.io/CsvHelper/
		{
			record = csv.GetRecords<T>().ToList();
		}
	}
}
