namespace MyChat.Razor;

public class CheepDTO
{
    public int CheepId { get; set; }
    public required string Author { get; set; }
    //public required string AuthorDTO Author { get; set; }
    public required string Cheep  { get; set; }
    public required DateTime Timestamp { get; set; }
}