using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Test.core.Services;
using Test.dataaccess;
using Test.model.Users;

namespace Test.consoleapp
{
    class Program
    {
        public static void Main(string[] args) => MainAsync().GetAwaiter().GetResult();

        private static async Task MainAsync()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true) //override locally, gitignored
                .AddEnvironmentVariables();

            var configuration = builder.Build();

            // set up our DI same as in web
            var services = new ServiceCollection();
            services.AddOptions();
            
            services.AddDbContext<AuthDbContext>(options =>
               options.UseSqlServer(configuration.GetConnectionString("AuthDb")));
            // This does not work
            // services.AddIdentity<ApplicationUser, IdentityRole>()
            //     .AddEntityFrameworkStores<AuthDbContext>();
                
            services.AddLogging(logging =>
            {
                logging.AddConsole();
                logging.AddDebug();
            });

            services.AddScoped<IRegisterService, RegisterService>();

            var serviceProvider = services.BuildServiceProvider();

            var regService = serviceProvider.GetService<IRegisterService>();
            try {
                await regService.EnsureRoles();
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
