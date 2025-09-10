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
}