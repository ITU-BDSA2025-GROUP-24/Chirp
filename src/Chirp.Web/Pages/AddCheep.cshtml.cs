using System.ComponentModel.DataAnnotations;
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
           

        // Handle POST request
        public async Task OnPostAsync(String username, String email, String cheep)
        {
            await _repository.CreateCheep(username, email, cheep);
        }
    }
}