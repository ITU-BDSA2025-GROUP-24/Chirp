using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class ForgetMe : PageModel
{
    private readonly IAuthorRepository _authorRepo;

    public ForgetMe(IAuthorRepository authorRepo)
    {
        _authorRepo = authorRepo;
    }
    
    public IActionResult OnGet()
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToPage("/Public");
        }
            
        return Page();
    }
    
    public async Task<IActionResult> OnPostAsync()
    {
        if (User.Identity == null || !User.Identity.IsAuthenticated)
        {
            return RedirectToPage("/Public");
        }

        var authorName = User.Identity.Name;

        try
        {
            //Delete author/user from database 
            await _authorRepo.DeleteAuthor(authorName);

            //Sign user out
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
        catch (UserNotFound)
        {
            //User not exist? Sign them out (They still have a valid authentication cookie/session they must be signed out from)
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
        
        return RedirectToPage("/Public");
    }
}
