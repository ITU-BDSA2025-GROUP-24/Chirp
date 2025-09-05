using DocoptNet;
using Chirp.CLI; // if Parser/StoreCheep live here

const string Usage = @"Chirp.

Usage:
  chirp read
  chirp post <message>...
  chirp (-h | --help)
  chirp --version

Options:
  -h --help     Show help.
  --version     Show version.
";

var arguments = new Docopt().Apply(Usage, args, version: "Chirp 1.0", exit: true);

if (arguments["read"].IsTrue)
{
    var cheeps = Parser.ComposeCheep();
    foreach (var c in cheeps)
    {
        UserInterface.WriteOutCheep(c);
    }
}
else if (arguments["post"].IsTrue)
{
    String message = arguments["<message>"].ToString(); // single arg
    Parser.StoreCheep(message);
    Console.WriteLine("Cheep saved.");
}
/*
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
    var cheeps = Parser.ComposeCheep();
    foreach (var c in cheeps)
    {
       UserInterface.WriteOutCheep(c);
    }
}

void cheep(string cheeping) { Parser.StoreCheep(cheeping); }
*/