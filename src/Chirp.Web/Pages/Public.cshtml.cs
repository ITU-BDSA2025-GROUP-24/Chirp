using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Core;

namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepRepository _repository;
    public required IEnumerable<CheepDTO> Cheeps { get; set; }
    public AddCheepModel AddCheepModel{ get; set; }

    public PublicModel(ICheepRepository repository)
    {
        _repository = repository;
        AddCheepModel = new AddCheepModel(repository);
    }

    public async Task<ActionResult> OnGet()
    {
        Cheeps = await _repository.ReadCheep();
        return Page();
        
    }
    
    [BindProperty]
    public string newCheep { get; set; }
    public async Task OnPostAsync(string username, string email)
    {
        await AddCheepModel.OnPostAsync(username, email, newCheep);
    }
}