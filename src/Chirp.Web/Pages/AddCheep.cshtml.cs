using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Infrastructure;
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

        public async Task OnPostAsync(String Author, String Cheep)
        {
            var cheepDto = new CheepDTO 
            { 
                Author = Author, 
                Cheep = Cheep,  // or Message, depending on your DTO property name
                Timestamp = DateTime.Now
            };
    
            await _repository.CreateCheep(cheepDto);
        }
    }

}