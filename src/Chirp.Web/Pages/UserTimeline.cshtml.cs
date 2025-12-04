using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Core;
using Chirp.Infrastructure;


namespace Chirp.Web.Pages;


public class UserTimelineModel : PageModel
{
    private readonly ICheepRepository _repository;
    private readonly IAuthorRepository _authorRepository;
    public required IEnumerable<CheepDTO> Cheeps { get; set; }
    public bool DoesUserExist { get; set; } 
    
    /*
    Used in UserTimeline.cshtml if author does not exist,
    since cheep.Author.Name will not work if the user does not exist.
    */
    public string AuthorName { get; set; }
    
    public UserTimelineModel(ICheepRepository repository, IAuthorRepository authorRepository)
    {
        _repository = repository;
        _authorRepository = authorRepository;
    }

    public async Task<IActionResult> OnGet(string author, int i = 1)
    {
        var AuthorName = author; 
        
        try
        {
            var page = i < 1 ? 1 : i;
            
            var profileAuthor = await _authorRepository.GetAuthorByName(author);
            
            IEnumerable<CheepDTO> cheepsForPage;
            
            if (User.Identity?.IsAuthenticated == true &&
                string.Equals(User.Identity.Name, profileAuthor.Name, StringComparison.OrdinalIgnoreCase))
            {
                var followingIds = await _authorRepository.ReturnFollowing(profileAuthor.Name);
                followingIds.Add(profileAuthor.AuthorId);

                cheepsForPage = await _repository.ReadCheepForAuthors(page, followingIds);
            }
            else
            {
                cheepsForPage = await _repository.ReadCheep(page, profileAuthor.Name);
            }

            Cheeps = cheepsForPage;
            DoesUserExist = true;
        }
        catch (UserNotFound)
        {
            DoesUserExist = false;
            Cheeps = new List<CheepDTO>();
        }

        return Page();
    }
}
