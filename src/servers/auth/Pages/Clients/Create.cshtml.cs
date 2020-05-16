using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Test.auth.Services;

namespace Test.auth.Pages.Clients
{
    public class CreateModel : PageModel
    {
        private readonly ITestClientsService _clientService;

        public CreateModel(ITestClientsService service)
        {
            _clientService = service;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public string ClientId { get; set; }

        [BindProperty]
        public string Secret { get; set; }
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Secret = await _clientService.Create(ClientId);

            return Page();
        }
    }
}
