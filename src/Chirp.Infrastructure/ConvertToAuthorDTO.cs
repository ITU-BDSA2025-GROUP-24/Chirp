using System.ComponentModel.DataAnnotations;
using Chirp.Core;

namespace Chirp.Infrastructure;

public static class ConvertToAuthorDTO
{
    public static AuthorDTO ToAuthorDTO(this Author author)
    {
        var authorDTO = new AuthorDTO()
        {
            AuthorId = author.AuthorId,
            Name = author.Name,
            Email = author.Email
        };
        
        return authorDTO;
    }
}