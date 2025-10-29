using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Core;

namespace Chirp.Web.Pages;


public class UserTimelineModel : PageModel
{
    private readonly ICheepRepository _repository;
    public required IEnumerable<CheepDTO> Cheeps { get; set; }

    public UserTimelineModel(ICheepRepository repository)
    {
        _repository = repository;
    }

    public async Task<ActionResult> OnGet(string author)
    {
        Cheeps = await _repository.ReadCheep(1,author);
        return Page();
    }
}
