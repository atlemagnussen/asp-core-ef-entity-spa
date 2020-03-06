using Microsoft.AspNetCore.Identity;

namespace Test.auth.Models
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsEnabled { get; set; }
    }
}
