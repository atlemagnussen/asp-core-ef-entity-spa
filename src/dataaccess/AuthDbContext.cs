using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Test.model.Users;

namespace Test.dataaccess
{
    public class AuthDbContext : IdentityDbContext<ApplicationUser>
    {
        public AuthDbContext(
            DbContextOptions<AuthDbContext> options) : base(options)
        {
        }
    }
}
