using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class UserInformation : PageModel
{
    public readonly IAuthorRepository _authorRepo; 
    public readonly ICheepRepository _cheepRepo;
    private readonly IProfileImageStorage _imageStorage;
    public IEnumerable<CheepDTO> Cheeps { get; set; }
    public AuthorDTO Author { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string ProfileImageUrl { get; set; }
    public List<string> Following { get; set; } = new List<string>();
   
    
    public UserInformation(IAuthorRepository authorRepo, ICheepRepository cheepRepo,IProfileImageStorage imageStorage)
    {
        _authorRepo = authorRepo;
        _cheepRepo = cheepRepo;
        _imageStorage = imageStorage;
        Username = "[No username]";
        Email = "[No email]";
    }
    
   
    [BindProperty] 
    public IFormFile Upload { get; set; }
    
    public async Task CreateUserInfo(string author)
    {
        if (author == null)
        {
            return;
        }

        Author = await _authorRepo.GetAuthorByName(author);
        Username = Author.Name;
        ProfileImageUrl = Author.ProfileImageUrl;
        Console.WriteLine($"[CreateUserInfo] Loaded ProfileImageUrl: {ProfileImageUrl}");

        Console.WriteLine($"Registered username: {Username}");

        
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
            await CreateUserInfo(User.Identity.Name);
            
            Cheeps = await _cheepRepo.ReadCheep(page, User.Identity.Name);
        }
        catch (UserNotFound)
        {
            return RedirectToPage("/Public");
        }

        return Page();
    }
    
    public async Task<IActionResult> OnPostAsync()
    {
        Console.WriteLine($"[OnPost] Upload null? {Upload is null}, length: {Upload?.Length ?? 0}");

        if (!User.Identity?.IsAuthenticated ?? true)
            return Challenge();

        if (Upload is null || Upload.Length == 0)
            return RedirectToPage();

        var author = await _authorRepo.GetAuthorByName(User.Identity!.Name!);

        var imageUrl = await _imageStorage.UploadProfileImageAsync(
            Upload.OpenReadStream(),
            Upload.ContentType,
            Upload.FileName);

        author.ProfileImageUrl = imageUrl;
        await _authorRepo.UpdateAsync(author);

        return RedirectToPage();
    }
    
    
   
}

