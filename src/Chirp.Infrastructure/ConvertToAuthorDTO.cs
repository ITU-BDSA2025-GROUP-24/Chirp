using System.ComponentModel.DataAnnotations;
using Chirp.Core;

namespace Chirp.Infrastructure;

public static class ConvertToAuthorDTO
{
    public static AuthorDTO ToAuthorDTO(this Author author)
    {
        string _Email;
        if (author.Email == null)
        {
            _Email = author.Name + "@chirp.com";
        }
        else
        {
            _Email = author.Email;
        }
        var authorDTO = new AuthorDTO()
        {
            Name = author.Name,
            Email =  _Email,
            AuthorId = author.AuthorId,
            FollowsId = author.FollowsId
        };
        
        return authorDTO;
    }
}