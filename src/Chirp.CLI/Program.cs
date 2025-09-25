using Chirp.CLI;
using DocoptNet;
using System.Net.Http.Headers;
using System.Linq;
using SimpleDB;

class Program
{
    static IDatabaseRepository<Cheep> dbRepository;
    static void Main(string[] args)
    {
        const string Usage = @"Chirp.

Usage:
  chirp read
  chirp cheep <message>...
  chirp (-h | --help)
";
        
        dbRepository = CsvDatabase<Cheep>.getInstance();
        try
        {
            var arguments = new Docopt().Apply(Usage, args, version: "Chirp 2.0 (Web)", exit: true);

            if (arguments["read"].IsTrue)
            {
                dbRepository.Read();
            }
            else if (arguments["cheep"].IsTrue)
            {
                var list = arguments["<message>"].AsList;
                var message = string.Join(" ", list.Cast<object>().Select(o => o.ToString()));
                dbRepository.Store(cheep(message));
            }
        }
        catch (DocoptInputErrorException e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(Usage);
        }
    }


    static Cheep cheep(string message)
    {
        UserInterface.PostingCheep(message);

        var newCheep = new Cheep(
            Environment.UserName,
            message,
            DateTimeOffset.Now.ToUnixTimeSeconds()
        );
        return newCheep;
    }
}