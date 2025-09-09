
using System.Collections;
using System.Text;
using Chirp.CLI;

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
    Cheep hey = new Cheep();
}
void read()
{ 
    var cheeps = Parser.ComposeCheep(args);
    foreach (var c in cheeps)
    {
       UserInterface.WriteOutCheep(c);
    }
}

void cheep(string cheeping)
{
    StringBuilder s = new StringBuilder();
    string name = Environment.UserName;
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