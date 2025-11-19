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
    public int Page { get; set; } = 1;
    
    public int TotalPages { get; set; }
    
    public required IEnumerable<CheepDTO> Cheeps { get; set; }
    public AddCheepModel AddCheepModel{ get; set; }

    public PublicModel(ICheepRepository repository)
    {
        _repository = repository;
        AddCheepModel = new AddCheepModel(repository);
    }

    public async Task<IActionResult> OnGetAsync([FromQuery(Name = "page")] int page = 1)
    {
        CurrentPage = page < 1 ? 1 : page;
        Cheeps = await _repository.ReadCheep(CurrentPage);
        Console.WriteLine($"Page={CurrentPage}");
        return Page();
        
    }

    private async Task IdentityCheck()
    {
        if (User.Identity == null || User.Identity.Name == null)
        {
            return;
        }
        var username = User.Identity.Name;
        var email = User.Identity.Name + "@chirp.com";

        if (username == null)
        {
            return;
        }

        if (!await _authorRepository.UserExists(username, email))
        {
            await _authorRepository.CreateNewAuthor(username, email);
        }
    }
    

    [BindProperty]
    public string? NewCheep { get; set; }
    public async Task OnPostAddCheep()
    {
        //If any is empty then simply return instead of create cheep
        if (User.Identity == null || User.Identity.Name == null || NewCheep == null)
        {
            return; 
        }
        string username = User.Identity.Name;
        string email = User.Identity.Name + "@chirp.com";
        await AddCheepModel.OnPostAsync(username, email, NewCheep);
    }
}