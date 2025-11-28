using System.ComponentModel.DataAnnotations;
using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepRepository _repository;
    private readonly IAuthorRepository _authorRepository;

    public int CurrentPage { get; private set; } = 1;
    
    private AuthorDTO dto;

    public IEnumerable<CheepDTO> Cheeps { get; set; } = Enumerable.Empty<CheepDTO>();

    public IEnumerable<Guid> Followings { get; set; } = Enumerable.Empty<Guid>();

    public AddCheepModel AddCheepModel { get; set; }

    public PublicModel(ICheepRepository repository, IAuthorRepository authorRepository)
    {
        _repository = repository;
        _authorRepository = authorRepository;
        AddCheepModel = new AddCheepModel(repository);
    }
 
    public async Task<IActionResult> OnGetAsync([FromQuery(Name = "page")] int page = 1)
    {
        CurrentPage = page < 1 ? 1 : page;

        await IdentityCheck();

        Cheeps = await _repository.ReadCheep(CurrentPage);

        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            Followings = await _authorRepository.ReturnFollowing(User.Identity.Name);
        }
        
        return Page();
    }
    
    private async Task IdentityCheck()
    {
        if (User.Identity == null || User.Identity.Name == null)
        {

            return;
        }

        var username = User.Identity.Name;
        var email = username + "@chirp.com";

        if (!await _authorRepository.UserExists(username, email))
        {
 
            await _authorRepository.CreateNewAuthor(username, email);
        }

        dto = await _authorRepository.GetAuthorByName(username);
    }
    
    public async Task<bool> isFollowing(Guid authorId)
    {
        if (User.Identity == null || User.Identity.Name == null)
        {
            return false;
        }

        if (dto != null)
        {
            return dto.FollowsId.Contains(authorId);
        }

        var following = await _authorRepository.isFollowing(User.Identity.Name, authorId);
        return following;
    }
    
    public async Task<IActionResult> OnPostFollowAsync(Guid id, int page)
    {

        if (!(User.Identity?.IsAuthenticated ?? false))
        {
            return RedirectToPage("/Login");
        }

        CurrentPage = page < 1 ? 1 : page;

        if (await isFollowing(id))
        {
            await _authorRepository.UnFollowAsync(User.Identity!.Name!, id);

        }
        else
        {
            await _authorRepository.AddFollowAsync(User.Identity!.Name!, id);

        }
        return RedirectToPage("/Public", new { page = CurrentPage });
    }
     
    [BindProperty]
    public string Message { get; set; } = string.Empty;
    
    public async Task<IActionResult> OnPostAsync(int page)
    {
        CurrentPage = page < 1 ? 1 : page;

        await IdentityCheck();

        if (User.Identity == null || User.Identity.Name == null || string.IsNullOrWhiteSpace(Message))
        {
            // Re-display page if something is wrong
            Cheeps = await _repository.ReadCheep(CurrentPage);

            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                Followings = await _authorRepository.ReturnFollowing(User.Identity.Name);
            }

            return Page();
        }

        string username = User.Identity.Name;
        string email = username + "@chirp.com";

        try
        {
            await AddCheepModel.OnPostAsync(username, email, Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error when adding cheep: " + ex.Message);
            Cheeps = await _repository.ReadCheep(CurrentPage);
            if (User.Identity.IsAuthenticated)
            {
                Followings = await _authorRepository.ReturnFollowing(User.Identity.Name);
            }
            return Page();
        }
        
        return RedirectToPage("/Public", new { page = CurrentPage });
    }
}
