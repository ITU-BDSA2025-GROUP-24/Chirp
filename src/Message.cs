namespace MyChat.Razor;

public class Message
{
    public int MessageId { get; set; }
    public int UserId  { get; set; }
    public string Text { get; set; }
    public User User { get; set; }
}