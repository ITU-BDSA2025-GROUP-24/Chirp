using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class UserInformation : PageModel
{
    public readonly IAuthorRepository _authorRepo; 
    public readonly ICheepRepository _cheepRepo;
    public required IEnumerable<CheepDTO> Cheeps { get; set; }
    public required AuthorDTO Author { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public List<string> Following { get; set; } = new List<string>();
   
    
    public UserInformation(IAuthorRepository authorRepo, ICheepRepository cheepRepo)
    {
        _authorRepo = authorRepo;
        _cheepRepo = cheepRepo;
        Username = "[No username]";
        Email = "[No email]";
    }

    public async Task CreateUserInfo(string? author)
    {
        if (author == null)
        {
            return;
        }

        Author = await _authorRepo.GetAuthorByName(author);
        Username = Author.Name;

        Console.WriteLine($"Registered username: {Username}");


        //Set user email if registered author email is not null/exists (Should always be true...)
        if (Author.Email != null && !Email.Equals(""))
        {
            Email = Author.Email;
        }

        var followIds = await _authorRepo.ReturnFollowing(User.Identity.Name);

        foreach (var followId in followIds)
        {
            var authorDTO = await _authorRepo.GetAuthorById(followId);
            Following.Add(authorDTO.Name); 
        }
    }

    public async Task<IActionResult> OnGet(string author, int i = 1)
    {
        var page = i < 1 ? 1 : i;

        try
        {
            // Call CreateUserInfo to initialize user data
            await CreateUserInfo(User.Identity.Name);
            
            // Get cheeps for this user
            Cheeps = await _cheepRepo.ReadCheep(page, User.Identity.Name);
        }
        catch (UserNotFound)
        {
            // If user doesn't exist, redirect to public timeline
            return RedirectToPage("/Public");
        }

        return Page();
    }
    
    
   
}

