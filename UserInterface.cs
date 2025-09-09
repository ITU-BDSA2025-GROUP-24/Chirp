namespace Chirp.CLI;

public class UserInterface
{
    public static void WriteOutCheep(Cheep Cheep)
    {
        var when = DateTimeOffset.FromUnixTimeSeconds(Cheep.Timestamp)
            .ToLocalTime()
            .ToString("yyyy-MM-dd HH:mm");
        Console.WriteLine($"[{when}] {Cheep.Author}: {Cheep.Message}");
    }
}