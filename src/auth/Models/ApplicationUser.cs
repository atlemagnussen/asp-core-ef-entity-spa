using Microsoft.AspNetCore.Identity;

namespace Test.auth.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName {get; set; }
        public bool IsEnabled { get; set; }
    }
}
