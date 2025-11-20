using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Core;

namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepRepository _repository;
    
    public int CurrentPage { get; private set; } = 1;
    
    [BindProperty(SupportsGet = true)]
    public new int Page { get; set; } = 1;
    
    public int TotalPages { get; set; }
    
    public required IEnumerable<CheepDTO> Cheeps { get; set; }

    public PublicModel(ICheepRepository repository)
    {
        _repository = repository;
    }

    public async Task<IActionResult> OnGetAsync([FromQuery(Name = "page")] int page = 1)
    {
        CurrentPage = page < 1 ? 1 : page;
        Cheeps = await _repository.ReadCheep(CurrentPage);
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
           return Page();
        }
        
        var username = User.Identity.Name;
        var email = User.Identity.Name + "@chirp.com";
        
        try
        {
            await _repository.CreateCheep(username, email, Message);
            return RedirectToPage("/Public", new {page});
        }
        catch (Exception msgEx)
        {
            ModelState.AddModelError(string.Empty, $"Failed to create cheep: {msgEx.Message}");
            return Page();
        }
   }
}