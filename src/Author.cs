namespace MyChat.Razor;

public class Author
{
    public int AuthorID { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public ICollection<Cheep> Cheep { get; set; }
}