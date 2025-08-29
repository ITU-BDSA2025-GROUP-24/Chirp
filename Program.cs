string[] files = new string[10];
try
{
    files = File.ReadAllLines(@"chirp_cli_db.csv");
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
}

foreach (var file in files)
{
    string[] parts = file.Split(',');
    long tid = long.Parse(parts[2]);
    DateTime TidDato = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    TidDato = TidDato.AddSeconds(tid);
    TidDato = TidDato.AddMinutes(120);
    Console.WriteLine(parts[0] + " @ " + TidDato.ToShortDateString() + " "+ TidDato.ToLongTimeString() + " " + parts[1]);
}