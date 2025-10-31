   
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Chirp.Core;

    namespace Chirp.Web.Pages
    {
        public class AddCheepModel : PageModel
        {
            
            private readonly ICheepRepository _repository;

            [BindProperty]
            public AuthorDTO Author { get; set; }

            [BindProperty]
            public string Cheep { get; set; }

            public AddCheepModel(ICheepRepository repository)
            {
                _repository = repository;
            }

            // Handle GET request - set the author from query string
            public void OnGet(string author)
            {
                Author = new AuthorDTO()
                {
                    Name = author
                };
            }

            // Handle POST request
            public async Task<IActionResult> OnPostAsync()
            {
                if (Author == null || string.IsNullOrWhiteSpace(Author.Name) || string.IsNullOrWhiteSpace(Cheep))
                {
                    ModelState.AddModelError(string.Empty, "Author and Cheep are required");
                    return Page();
                }

                var cheepDto = new CheepDTO
                {
                    Author = Author,
                    Cheep = Cheep,
                    TimeStamp = DateTime.Now
                };

                await _repository.CreateCheep(cheepDto);
                return Redirect("/" + Author.Name);
            }
        }
    }