using Chirp.Core;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class RemoveCheep : PageModel
{
    private readonly ICheepRepository _cheepRepo;

    public RemoveCheep(ICheepRepository cheepRepo)
    {
        _cheepRepo = cheepRepo;
    }
    
    public async Task OnGetAsync(Guid cheepId, string username)
    {
        await _cheepRepo.DeleteCheep(cheepId, username);
    }
}
