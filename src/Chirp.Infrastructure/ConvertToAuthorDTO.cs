using System.ComponentModel.DataAnnotations;
using Chirp.Core;

namespace Chirp.Infrastructure;


// Converts author entities to AuthorDTOs 
public static class ConvertToAuthorDTO
{
    public static AuthorDTO ToAuthorDTO(this Author author)
    {
        string _Email;
        //If the author has no email, generate a default one using their name
        if (author.Email == null)
        {
            _Email = author.Name + "@chirp.com";
        }
        else
        {
            _Email = author.Email;
        }
        
        //Create and populate the AuthorDTO with author data
        var authorDTO = new AuthorDTO()
        {
            Name = author.Name,
            Email =  _Email,
            AuthorId = author.AuthorId,
            FollowsId = author.FollowsId,
            ProfileImageUrl = author.ProfileImageUrl
        };
        
        return authorDTO;
    }
}