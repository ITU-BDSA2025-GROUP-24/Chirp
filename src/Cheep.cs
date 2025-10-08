namespace MyChat.Razor;

public class Cheep
{
    public int CheepID { get; set; }
    public int AuthorID  { get; set; }
    public string Text { get; set; }
    public Author Author { get; set; }
    public Datetime Timestamp { get; set; }
}