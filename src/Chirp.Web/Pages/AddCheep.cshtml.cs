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
           
        
        public async Task OnPostAsync(string username, string email, string cheep)
        {
            await _repository.CreateCheep(username, email, cheep);
        }
    }
}