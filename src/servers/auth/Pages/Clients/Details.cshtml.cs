using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Test.model;

namespace Test.auth.Pages.Clients
{
    public class DetailsModel : PageModel
    {
        private readonly dataaccess.AuthDbContext _context;

        public DetailsModel(dataaccess.AuthDbContext context)
        {
            _context = context;
        }

        public ApiClient ApiClient { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ApiClient = await _context.ApiClients.FirstOrDefaultAsync(m => m.Id == id);

            if (ApiClient == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
