
using System.Globalization; 
using CsvHelper; 


namespace SimpleDB;

public class CsvDatabase<T> : IDatabaseRepository<T>
{
	private string filePath;

	public CsvDatabase(string filePath = "../../../chirp_cli_db.csv")
	{
		this.filePath = filePath;

		foreach (var property in typeof(T).GetProperties())
		{
			Console.WriteLine(property);
		}
	}

	public IEnumerable<T> Read(int? limit = null)
	{
		List<T>? rec;
		using (var reader = new StreamReader(filePath))
		using (var csv = new CsvReader(reader,
			       CultureInfo.InvariantCulture)) //From https://joshclose.github.io/CsvHelper/
		{
			rec = csv.GetRecords<T>().ToList();
		}

		if (limit != null && limit < rec.Count)
		{
			int n = limit.GetValueOrDefault();
			return rec.GetRange(rec.Count - n, n);
		}

		return rec;
	}

	public void Store(T record)
	{
		using (var writer = new StreamWriter(filePath))
		using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture)) //From https://joshclose.github.io/CsvHelper/
		{
			csv.WriteRecord(record);
			csv.NextRecord();
		}
	}
}
