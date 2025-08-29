string[] files = File.ReadAllLines(@"chirp_cli_db.csv");

foreach (var file in files)
{
    Console.WriteLine(file);    
}
