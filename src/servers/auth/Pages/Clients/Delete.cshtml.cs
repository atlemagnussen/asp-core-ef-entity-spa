using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Test.model;

namespace Test.auth.Pages.Clients
{
    public class DeleteModel : PageModel
    {
        private readonly dataaccess.AuthDbContext _context;

        public DeleteModel(dataaccess.AuthDbContext context)
        {
            _context = context;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ApiClient = await _context.ApiClients.FindAsync(id);

            if (ApiClient != null)
            {
                _context.ApiClients.Remove(ApiClient);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
