using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Reflection;
using System;
using Test.model.Users;
using Test.dataaccess.Data;
using Test.auth.Services;

namespace Test.auth.Extentions
{
    public static class ServiceCollectionExtension
    {
        public static void AddIdentityServerConfig(this IServiceCollection services, IConfiguration configuration)
        {
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            var connectionString = configuration.GetConnectionString("AuthDb");

            services.AddDbContext<AuthDbContext>(options =>
               options.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly)));

            //services.AddIdentity<ApplicationUser, IdentityRole>()
            //    .AddEntityFrameworkStores<ApplicationDbContext>()
            //    .AddDefaultTokenProviders();

            services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AuthDbContext>();

            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
                options.UserInteraction.LoginUrl = "/Account/Login";
                options.UserInteraction.LogoutUrl = "/Account/Logout";
                options.Authentication = new IdentityServer4.Configuration.AuthenticationOptions()
                {
                    CookieLifetime = TimeSpan.FromHours(10), // ID server cookie timeout set to 10 hours
                    CookieSlidingExpiration = true
                };
            });
            builder.AddDeveloperSigningCredential();

            builder.AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                    options.EnableTokenCleanup = true;
                });
            builder.AddInMemoryClients(Config.GetClients());
            builder.AddInMemoryApiResources(Config.GetApiResources());
            builder.AddInMemoryIdentityResources(Config.GetIdentityResources());
            builder.AddAspNetIdentity<ApplicationUser>();
            builder.AddProfileService<TestProfileService>();

            services.AddAuthentication()
                .AddAzureAD(options => configuration.Bind("AzureAd", options));
            services.Configure<OpenIdConnectOptions>(AzureADDefaults.OpenIdScheme, options =>
            {
                options.Authority = options.Authority + "/v2.0/";
                options.TokenValidationParameters.ValidateIssuer = true;
                options.SignInScheme = IdentityConstants.ExternalScheme;
            });

            //services.AddScoped<IProfileService, TestProfileService>();
        }
    }
}
