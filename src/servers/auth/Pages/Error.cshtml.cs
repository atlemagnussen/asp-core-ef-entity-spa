using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace Test.auth.Pages
{
    public class ErrorModel : PageModel
    {
        private readonly IIdentityServerInteractionService _interaction;

        public ErrorModel(IIdentityServerInteractionService interaction)
        {
            _interaction = interaction;
        }

        public string ErrorId { get; set; }
        public ErrorMessage Error { get; set; }
        
        public async Task<IActionResult> OnGetAsync(string errorId)
        {
            
            ErrorId = errorId;
            if (!string.IsNullOrEmpty(ErrorId))
                Error = await _interaction.GetErrorContextAsync(ErrorId);
            return Page();
        }
    }
}