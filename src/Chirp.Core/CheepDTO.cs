using System.ComponentModel.DataAnnotations;

namespace Chirp.Core;

public class CheepDTO
{
    public int CheepId { get; set; }
    public required AuthorDTO Author { get; set; }
    public required string Cheep  { get; set; }
    public required DateTime Timestamp { get; set; }
}