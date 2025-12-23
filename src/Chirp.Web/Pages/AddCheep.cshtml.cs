using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Core;

namespace Chirp.Web.Pages
{
    //Page model for handeling creation of new cheeps
    public class AddCheepModel : PageModel
    {
            
        private readonly ICheepRepository _repository;

        public AddCheepModel(ICheepRepository repository)
        {
            _repository = repository;
        }
           
        //POST request to create a new cheep
        public async Task OnPostAsync(string username, string email, string cheep)
        {
            await _repository.CreateCheep(username, email, cheep);
        }
    }
}