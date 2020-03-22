using Microsoft.AspNetCore.Identity;

namespace Test.model.Users
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public bool IsEnabled { get; set; }
    }
}
