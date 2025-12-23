using Chirp.Core;

namespace Chirp.Infrastructure;

// Converts cheep entities to AuthorDTOs 
public static class ConvertToCheepDTO
{
    public static CheepDTO ToCheepDTO(this Cheep cheep)
    {
        var cheepDTO = new CheepDTO
        {
            CheepId = cheep.CheepId,
            Cheep = cheep.Text,
            TimeStamp = cheep.TimeStamp,
            Author = ConvertToAuthorDTO.ToAuthorDTO(cheep.Author)
        };
        return cheepDTO;
    }
}  