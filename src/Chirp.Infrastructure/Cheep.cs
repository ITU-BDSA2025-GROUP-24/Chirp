using System.ComponentModel.DataAnnotations;

namespace Chirp.Infrastructure;

public class Cheep
{
    //Generates 128bit unique ID for cheep 
    [Required]
    public required Guid CheepId { get; set; }
    [Required]
    [MaxLength(160, ErrorMessage = "Cheep cannot be longer than 160 characters!")]
    public required string Text { get; set; }
    [Required]
    public required Author Author { get; set; }
    [Required]
    public required DateTime TimeStamp { get; set; }
}
