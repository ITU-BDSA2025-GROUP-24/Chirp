using Chirp.Core;

namespace Chirp.Infrastructure;

public static class ConvertToCheepDTO
{
    public static CheepDTO ToCheepDTO(this Cheep cheep)
    {
        var cheepDTO = new CheepDTO
        {
            CheepId = cheep.CheepId,
            Author = ConvertToAuthorDTO.ToAuthorDTO(cheep.Author),
            Cheep = cheep.Text,
            Timestamp = cheep.Timestamp
        };
        return cheepDTO;
    }
}  