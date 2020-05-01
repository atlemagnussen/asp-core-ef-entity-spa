using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Test.auth.Services;

namespace Test.auth.Components
{
    public class AdminOverViewViewComponent : ViewComponent
    {
        private readonly IAdminService _adminService;

        public AdminOverViewViewComponent(IAdminService adminService)
        {
            _adminService = adminService;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var vm = await _adminService.GetAdminViewModel();
            return View(vm);
        }
    }
}
