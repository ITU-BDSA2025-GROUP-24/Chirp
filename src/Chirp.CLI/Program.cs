using Chirp.CLI;
using DocoptNet;
using System.Net.Http.Headers;
using System.Linq;

const string Usage = @"Chirp.

Usage:
  chirp read
  chirp cheep <message>...
  chirp (-h | --help)
";

try
{
    var arguments = new Docopt().Apply(Usage, args, version: "Chirp 2.0 (Web)", exit: true);

    var baseUrl = Config.BaseUrl;
    using var http = new HttpClient { BaseAddress = new Uri(baseUrl) };
    http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    var api = new ChirpApi(http);

    if (arguments["read"].IsTrue)
    {
        var cheeps = await api.GetAllAsync();
        if (cheeps.Count == 0)
        {
            UserInterface.NoCheeps();
        }
        else
        {
            foreach (var c in cheeps)
                UserInterface.WriteOutCheep(c);
        }
    }
    else if (arguments["cheep"].IsTrue)
    {
        var list = arguments["<message>"].AsList;          // returns ArrayList (non-generic)
        var message = string.Join(" ",                     // Cast to IEnumerable<object> then ToString
            list.Cast<object>().Select(o => o.ToString()));

        var Author = Environment.UserName;
        var Message = message;
        var Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        var cheepa = new Cheep(Author, Message, Timestamp);

        var ok = await api.PostAsync(cheepa);
        Console.WriteLine(ok ? "Posted." : "Failed.");
        return;
    }
}
catch (DocoptInputErrorException e)
{
    Console.WriteLine(e.Message);
    Console.WriteLine(Usage);
}

void cheep(string message, ChirpApi api)
{
    UserInterface.PostingCheep(message);

    var newCheep = new Cheep(
        Environment.UserName,
        message,
        DateTimeOffset.Now.ToUnixTimeSeconds()
    );

    var ok = api.PostAsync(newCheep).GetAwaiter().GetResult();
    Console.WriteLine(ok ? "Posted." : "Failed.");
}
