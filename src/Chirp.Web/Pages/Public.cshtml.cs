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

    [BindProperty]
    public string newCheep { get; set; }
    public async Task OnPostAsync(string username, string email)
    {
        await AddCheepModel.OnPostAsync(username, email, newCheep);
    }
}