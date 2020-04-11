using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Test.dataaccess
{
    public static class DataAccessServiceCollectionExtension
    {
        public static void AddCommonDataProtection(this IServiceCollection services, IConfiguration configuration)
        {
            //var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            var connectionString = configuration.GetConnectionString("AuthDb");

            services.AddDbContext<DataProtectionDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddDataProtection()
                .PersistKeysToDbContext<DataProtectionDbContext>()
                .SetApplicationName("asp-core-ef-is4-spa");
        }
    }
}
