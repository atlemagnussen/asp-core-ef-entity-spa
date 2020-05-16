using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Test.dataaccess;
using Test.model;

namespace Test.auth.Pages.Clients
{
    public class IndexModel : PageModel
    {
        private readonly AuthDbContext _context;

        public IndexModel(AuthDbContext context)
        {
            _context = context;
        }

        public IList<ApiClient> ApiClient { get;set; }

        public async Task OnGetAsync()
        {
            ApiClient = await _context.ApiClients.ToListAsync();
        }
    }
}
