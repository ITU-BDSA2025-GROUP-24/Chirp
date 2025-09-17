
using Chirp.CLI;
using SimpleDB;
using DocoptNet;

IDatabaseRepository<Cheep> dbRepository = new CsvDatabase<Cheep>();

const string Usage = @"Chirp.

Usage:
  chirp read
  chirp cheep <message>...
  chirp (-h | --help)
";

try
{
    var argsDict = new Docopt().Apply(Usage, args, exit: true);

    if (argsDict["read"].IsTrue)
    {
        //Reads and prints cheeps from database
        read(); 
        
    }
    else if (argsDict["cheep"].IsTrue)
    {
		//Adds cheep to database
        var message = string.Join(" ", argsDict["<message>"].AsList.Cast<string>());
        cheep(message);
    }
}
catch (DocoptInputErrorException e)
{
    Console.Error.WriteLine(e.Message);
    Console.Error.WriteLine(Usage);
    Environment.Exit(1);
}

void read()
{
	UserInterface.ReadingCheeps();
    var cheeps = dbRepository.Read();
    
    //Are there any recorded cheeps?
    if (!cheeps.Any())
    {
        UserInterface.NoCheeps();
        return;
    }

   	foreach (var cheep in cheeps)
	{
        UserInterface.WriteOutCheep(cheep);
    }
}

void cheep(string message)
{
	UserInterface.PostingCheep(message); 
    
    //Create and store new cheep using SimpeDB
    var newCheep = new Cheep(
		Environment.UserName, 
		message, 
		DateTimeOffset.Now.ToUnixTimeSeconds()
	);

    dbRepository.Store(newCheep);
}