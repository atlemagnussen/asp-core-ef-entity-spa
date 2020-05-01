using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Test.auth.Services;

namespace Test.auth.Components
{
    public class TopMenuViewComponent : ViewComponent
    {
        private readonly IAdminService _adminService;

        public TopMenuViewComponent(IAdminService adminService)
        {
            _adminService = adminService;
        }
        public async Task<IViewComponentResult> InvokeAsync(ClaimsPrincipal user)
        {
            var vm = await _adminService.GetTopMenuViewModel(user);
            return View(vm);
        }
    }
}
