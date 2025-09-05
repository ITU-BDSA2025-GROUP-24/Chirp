
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
    Parser.StoreCheep(cheeping);
}