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
    public string AuthorName { get; set; } //Used in UserTimeline.cshtml if author does not exist

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
            // normalize page
            var page = i < 1 ? 1 : i;

            // who is the profile we are visiting?
            var profileAuthor = await _authorRepository.GetAuthorByName(author);

            // default behavior: show ONLY this profile's cheeps
            IEnumerable<CheepDTO> cheepsForPage;

            // Are we logged in AND is this our own profile?
            if (User.Identity?.IsAuthenticated == true &&
                string.Equals(User.Identity.Name, profileAuthor.Name, StringComparison.OrdinalIgnoreCase))
            {
                // This is *my* own timeline → show me + people I follow
                var followingIds = await _authorRepository.ReturnFollowing(profileAuthor.Name);
                followingIds.Add(profileAuthor.AuthorId); // include myself

                cheepsForPage = await _repository.ReadCheepForAuthors(page, followingIds);
            }
            else
            {
                // Visiting someone else's profile → only their cheeps
                cheepsForPage = await _repository.ReadCheep(page, profileAuthor.Name);
            }

            Cheeps = cheepsForPage;
            //User exists, therefore true 
            DoesUserExist = true;
        }
        catch (UserNotFound)
        {
            //If user does not exist
            DoesUserExist = false;
            Cheeps = new List<CheepDTO>();
        }

        return Page();
    }
}
