using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Test.dataaccess
{
    public static class DataAccessServiceCollectionExtension
    {
        public static void AddCommonDataProtection(this IServiceCollection services)
        {
            services.AddDataProtection()
                .PersistKeysToDbContext<DataProtectionDbContext>()
                .SetApplicationName("asp-core-ef-is4-spa");
        }
        public static void AddCommonIdentitySettings(this IServiceCollection services)
        {
            // password options
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
            });
        }
    }
}
