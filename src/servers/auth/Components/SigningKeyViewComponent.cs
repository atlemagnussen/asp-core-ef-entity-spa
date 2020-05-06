using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Test.auth.Models;

namespace Test.auth.Components
{
    public class SigningKeyViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(SigningKeyModel vm)
        {
            return View(vm);
        }
    }
}
