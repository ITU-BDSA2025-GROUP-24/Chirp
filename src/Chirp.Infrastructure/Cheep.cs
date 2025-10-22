using System.ComponentModel.DataAnnotations;

namespace Chirp.Infrastructure;

public class Cheep
{
    public int CheepId { get; set; }
    [Required]
    public required int AuthorId  { get; set; }
    [Required]
    [StringLength(500)]
    public required string Text { get; set; }
    [Required]
    public required Author Author { get; set; }
    [Required]
    public required DateTime TimeStamp { get; set; }
}