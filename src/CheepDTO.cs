using System.ComponentModel.DataAnnotations;

namespace MyChat.Razor;

public class CheepDTO
{
    public int CheepId { get; set; }
    [Required]
    public required string Author { get; set; }
    [Required]
    //public required string AuthorDTO Author { get; set; }
    public required string Cheep  { get; set; }
    [Required]
    public required DateTime Timestamp { get; set; }
}