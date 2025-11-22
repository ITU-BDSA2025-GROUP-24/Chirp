using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Core;

namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepRepository _repository;

    private readonly IAuthorRepository _authorRepository;
    
    public int CurrentPage { get; private set; } = 1;
    
    [BindProperty(SupportsGet = true)]
    public new int Page { get; set; } = 1;
    
    public int TotalPages { get; set; }
    
    public required IEnumerable<CheepDTO> Cheeps { get; set; }
    
    public required IEnumerable<Guid> Followings { get; set; }
    public AddCheepModel AddCheepModel{ get; set; }

    public PublicModel(ICheepRepository repository, IAuthorRepository authorRepository) 
    {
        _repository = repository;
        AddCheepModel = new AddCheepModel(repository);
        _authorRepository = authorRepository;
    }

    public async Task<IActionResult> OnGetAsync([FromQuery(Name = "page")] int page = 1)
    {
        CurrentPage = page < 1 ? 1 : page;
        Cheeps = await _repository.ReadCheep(CurrentPage);
        await IdentityCheck();
        if (User.Identity.IsAuthenticated) {
            Followings = await _authorRepository.ReturnFollowing(User.Identity.Name);
        }
        Console.WriteLine($"Page={CurrentPage}");
        return Page();
    }

    [BindProperty]
    public string Message { get; set; }
    public async Task<IActionResult> OnPostAsync([FromQuery(Name = "page")] int page = 1)
   {
        //If any is empty then simply return instead of create cheep
        if (User.Identity == null || User.Identity.Name == null || string.IsNullOrWhiteSpace(Message))
        {
            Console.WriteLine("User.Identity is null");
            Console.WriteLine("User.Identity is null");
            Console.WriteLine("User.Identity is null");
            return;
        }
        
        var username = User.Identity.Name;
        var email = User.Identity.Name + "@chirp.com";

        if (username == null)
        {
            Console.WriteLine("User.Identity.Name is null");
            Console.WriteLine("User.Identity.Name is null");
            Console.WriteLine("User.Identity.Name is null");
            return;
        }

        if (!await _authorRepository.UserExists(username, email))
        {
            Console.WriteLine($"User {username} does not exist");
            Console.WriteLine($"User {username} does not exist");
            Console.WriteLine($"User {username} does not exist");
            Console.WriteLine($"User {username} does not exist");
            Console.WriteLine($"User {username} does not exist");
            Console.WriteLine($"User {username} does not exist");
            await _authorRepository.CreateNewAuthor(username, email);
        }
        Console.WriteLine($"User {username} exists");
        Console.WriteLine($"User {username} exists");
        Console.WriteLine($"User {username} exists");
        Console.WriteLine($"User {username} exists" + " With id " + _authorRepository.GetAuthorByName(username).Result.AuthorId);
    }
    
    public async Task<Boolean> isFollowing(Guid AuthorId)
    {
        Boolean following = await _authorRepository.isFollowing(User.Identity.Name,AuthorId);
        Console.WriteLine(following + " "+ User.Identity.Name + " following "  + AuthorId );
        return following;
    }
    
    public async Task<IActionResult> OnPostFollowAsync(Guid id, int page)
    {
        Console.WriteLine($"OnPostFollowAsync HIT: id={id}, page={page}");

        if (!User.Identity?.IsAuthenticated ?? true)
        {
            return RedirectToPage("/Login");
        }

        if (await isFollowing(id))
        {
            await _authorRepository.UnFollowAsync(User.Identity!.Name!, id);
            Console.WriteLine("Unfollowed");
        }
        else
        {
            await _authorRepository.AddFollowAsync(User.Identity!.Name!, id);
            Console.WriteLine("Followed");
        }

        // Keep the page number
        CurrentPage = page < 1 ? 1 : page;

        Console.WriteLine("REDIRECTING TO GET with page=" + CurrentPage);

        // This will run OnGetAsync([FromQuery] page=CurrentPage) and refill Cheeps
        return RedirectToPage("/Public", new { page = CurrentPage });
    }
    

    [BindProperty]
    public string Message { get; set; }
    public async Task OnPostAddCheep()
    {
        //If any is empty then simply return instead of create cheep
        if (User.Identity == null || User.Identity.Name == null || Message == null)
        {
            ModelState.AddModelError(string.Empty, $"Failed to create cheep: {msgEx.Message}");
            return Page();
        }
   }
}