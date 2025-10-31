   
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Chirp.Core;

    namespace Chirp.Web.Pages
    {
        public class AddCheepModel : PageModel
        {
            private readonly ICheepRepository _repository;

            public AddCheepModel(ICheepRepository repository)
            {
                _repository = repository;
            }

            public async Task OnPostAsync(String name, String cheep)
            {
                await _repository.CreateCheep(name, cheep);
            }
        }
    }
       