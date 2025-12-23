using Chirp.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

//Handels cheep deletion
public class RemoveCheep : PageModel
{
    private readonly ICheepRepository _cheepRepo;

    public RemoveCheep(ICheepRepository cheepRepo)
    {
        _cheepRepo = cheepRepo;
    }
    
    public IActionResult OnGet()
    {
        return Page();
    }
    
    //Handles POST requests for deleting a cheep
    public async Task<IActionResult> OnPostAsync(Guid cheepId, string returnUrl = "/")
    {
       if (!(User.Identity?.IsAuthenticated ?? false) || User.Identity.Name == null)
        {
            return RedirectToPage("/Public");
        }

        await _cheepRepo.DeleteCheep(cheepId, User.Identity.Name);
        
        if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
        {
            returnUrl = "/";
        }

        return LocalRedirect(returnUrl);
    }
}