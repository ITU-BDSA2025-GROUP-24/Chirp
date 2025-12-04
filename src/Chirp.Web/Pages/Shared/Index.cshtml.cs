using Chirp.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Chirp.Web.Pages.Shared;

public class IndexViewComponent : ViewComponent
{
    private IMemoryCache _cache;
    private IAuthorRepository _authorRepository;
    public IndexViewComponent(IMemoryCache cache, IAuthorRepository authorRepository)
        {
        _cache = cache;
        _authorRepository = authorRepository;
        }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        if (User.Identity == null || !User.Identity.IsAuthenticated || User.Identity.Name == null)
        {
            return View(); 
        }
        
        bool doesUsernameExist = await _authorRepository.UserExists(User.Identity.Name);
        if (!doesUsernameExist)
        {
            await _authorRepository.CreateNewAuthor(User.Identity.Name, User.Identity.Name + "@chirp.com");
        }

        AuthorDTO newAuthorDTO = await _authorRepository.GetAuthorByName(User.Identity.Name);
        
        return View();
    }
    
}
