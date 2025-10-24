namespace Chirp.Core;

public class AuthorDTO
{
    public int AuthorId { get; set; }
    public required string Name { get; set; }
    public string Email { get; set; }
}
