
using System.Globalization; 
using System.Reflection; 
using System.Reflection.Metadata;
using CsvHelper; 


namespace SimpleDB;

public class CSVDatabase<T> : IDatabaseRepository<T>
{
	string filePath; 
	
	public CSVDatabase(string filePath = "../../chirp_cli_db.csv")
	{
		this.filePath = filePath; 
		
		foreach(Property in prop in typeof(T).GetProperties())
		{
			
		}	
    }
	
	public IEnumerable<T> Read(int? limit = null)
	{
	  
	}
}