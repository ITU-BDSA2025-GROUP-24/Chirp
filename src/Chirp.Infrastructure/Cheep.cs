using System.ComponentModel.DataAnnotations;

namespace Chirp.Infrastructure;

public class Cheep
{
    public int CheepId { get; set; }
    [Required]
    [MaxLength(160, ErrorMessage = "Cheep cannot be longer than 160 characters!")]
    public required string Text { get; set; }
    [Required]
    public required Author Author { get; set; }
    [Required]
    public int AuthorId { get; set; }
    public required DateTime TimeStamp { get; set; }
}
