
using System.Text;

string[] files = new string[10];
try
{
    files = File.ReadAllLines(@"chirp_cli_db.csv");
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
}

if (args[0] == "read")
{
    read();
} else if (args[0] == "cheep")
{
    cheep(args[1]);
}
void read()
{
    foreach (var file in files)
    {
        string[] bigParts = file.Split('"');
        bigParts[0] = bigParts[0].Split(',')[0];
        bigParts[2] = bigParts[2].Split(',')[1];
        string[] parts = bigParts;
        long tid = long.Parse(parts[2]);
        DateTime TidDato = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        TidDato = TidDato.AddSeconds(tid);
        TidDato = TidDato.AddMinutes(120);
        Console.WriteLine(parts[0] + " @ " + TidDato.ToShortDateString() + " "+ TidDato.ToLongTimeString() + " " + parts[1]);
    }
}

void cheep(string cheeping)
{
    StringBuilder s = new StringBuilder();
    string name = System.Security.Principal.WindowsIdentity.GetCurrent().Name + " ";
    String tidDato = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + " ";
    s.Append(name + " @ " + tidDato + cheeping);
    String newCheep =  s.ToString();
    Console.WriteLine(newCheep);
    StreamWriter sw = new StreamWriter(@"chirp_cli_db.csv",true);
    s.Clear();

    long time = DateTimeOffset.Now.ToUnixTimeSeconds();
    s.Append(name + "," + '"' + cheeping + '"' + "," +time);
    sw.WriteLine(s.ToString());
    sw.Close();
}