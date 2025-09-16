namespace Chirp.CLI;

public class UserInterface
{
    public static void WriteOutCheep(Cheep cheep)
    {
        var when = DateTimeOffset.FromUnixTimeSeconds(cheep.Timestamp)
            .ToLocalTime()
            .ToString("yyyy-MM-dd HH:mm");
        Console.WriteLine($"[{when}] {cheep.Author}: {cheep.Message}");
    }

	//Instructions for user
	public static void Instructions()
	{
		Console.WriteLine("Please 'read' or 'cheep'");
	}

	//Message telling user that cheeps are being read
	public static void ReadingCheeps()
	{
		Console.WriteLine("Reading cheeps...");
	}

	//Message telling user that their cheep is being cheeped 
	public static void PostingCheep(string message) 
	{
		Console.WriteLine($"Cheeping: {message}");
	}

	//If there are no cheeps
	public static void NoCheeps()
	{
		Console.WriteLine("No cheeps yet");
	}
}