
using System.Collections;
using Chirp.CLI;
using SimpleDB;
using DocoptNet;
IDatabaseRepository<Cheep> dbRepository = new CsvDatabase<Cheep>();
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

try
{
    var argsDict = new Docopt().Apply(Usage, args, version: "Chirp 1.0", exit: true);

    if (argsDict["read"].IsTrue)
    {
        // TODO: load and print cheeps from your DB
        Console.WriteLine("Reading cheeps...");
        // e.g., var cheeps = Parser.ComposeCheep(); foreach (var c in cheeps) UserInterface.WriteOutCheep(c);
    }
    else if (argsDict["post"].IsTrue)
    {
        var parts = ((IEnumerable)argsDict["<message>"].AsList)
            .Cast<object>()
            .Select(o => o?.ToString() ?? "");
        var message = string.Join(" ", parts);
        Console.WriteLine($"Posting: {message}");
        cheep(message);
        // TODO: save the cheep, e.g., Parser.StoreCheep(message);
    }
}
catch (DocoptInputErrorException e)
{
    Console.Error.WriteLine(e.Message);
    Console.Error.WriteLine(Usage);
    Environment.Exit(1);
}

//dbRepository - 'Link'/representation of database 
if (args.Length == 0)
{
    Console.WriteLine("Please 'read' or 'cheep'");
    return;
}

if (args[0] == "read")
{
    read();
}
else if (args[0] == "cheep")
{
    cheep(args[1]);
}

void read()
{
    var cheeps = dbRepository.Read();
    
    //Are there any recorded cheeps?
    if (!cheeps.Any())
    {
        Console.WriteLine("No cheeps yet");
        return;
    }

    foreach (var c in cheeps)
    {
        UserInterface.WriteOutCheep(c);
    }
}

void cheep(string cheeping)
{
    string name = Environment.UserName;
    long time = DateTimeOffset.Now.ToUnixTimeSeconds();
    
    //Create and store new cheep using SimpeDB
    var newCheep = new Cheep(name, cheeping, time);
    dbRepository.Store(newCheep);
}

/*
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
}*/