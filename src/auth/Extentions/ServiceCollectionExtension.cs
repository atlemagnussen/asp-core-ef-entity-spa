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
using Test.auth.Services;
using Test.dataaccess;
using IdentityServer4;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Azure.Identity;
using Azure.Security.KeyVault.Keys;

namespace Test.auth.Extentions
{
    public static class ServiceCollectionExtension
    {
        public static void AddIdentityServerConfig(this IServiceCollection services, 
            IConfiguration configuration, 
            IWebHostEnvironment environment)
        {
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            var connectionString = configuration.GetConnectionString("AuthDb");

            services.AddDbContext<DataProtectionDbContext>(options =>
                options.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly)));
                
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
                options.Events.RaiseErrorEvents = false;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
                options.UserInteraction.LoginUrl = "/Login";
                options.UserInteraction.LogoutUrl = "/Logout";
                options.UserInteraction.ErrorUrl = "/Error";
                options.Authentication = new IdentityServer4.Configuration.AuthenticationOptions()
                {
                    CookieLifetime = TimeSpan.FromHours(10), // ID server cookie timeout set to 10 hours
                    CookieSlidingExpiration = true
                };
            });
            //if (environment.IsDevelopment())
            //    builder.AddDeveloperSigningCredential();
            //else
            //{
            //IAzureKeyService service = new AzureKeyService(environment, configuration, null);
            //var keys = service.GetSigningKeys();
            //if (keys != null)
            //{
            //    if (keys.Current != null && keys.Current.Key != null)
            //        builder.AddSigningCredential(keys.Current.Key, keys.Current.Algorithm);
                
            //    if (keys.Previous != null && keys.Previous.Key != null)
            //        builder.AddValidationKey(keys.Previous.Key, keys.Previous.Algorithm);

            //    if (keys.Future != null && keys.Future.Key != null)
            //        builder.AddValidationKey(keys.Future.Key, keys.Future.Algorithm);
            //}
            
            //var rsaKeys = service.GetRsaSigningKeys();
            //builder.AddValidationKey(rsa.Key, rsa.Algorithm);
            //}
            builder.AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                    options.EnableTokenCleanup = true;
                });
            builder.AddInMemoryClients(Config.GetClients(configuration.GetValue<string>("AllowedClientUrl")));
            builder.AddInMemoryApiResources(Config.GetApiResources());
            builder.AddInMemoryIdentityResources(Config.GetIdentityResources());
            builder.AddAspNetIdentity<ApplicationUser>();
            builder.AddProfileService<TestProfileService>();

            services.AddAuthentication()
                .AddAzureAD(options =>
                {
                    options.Instance = configuration.GetValue<string>("AzureAd:Instance");
                    options.ClientId = configuration.GetValue<string>("AzureAd:ClientId");
                    options.TenantId = configuration.GetValue<string>("AzureAd:TenantId");
                })
                .AddGoogle(options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                    options.ClientId = configuration.GetValue<string>("Google:ClientId");
                    options.ClientSecret = configuration.GetValue<string>("Google:ClientSecret");
                });
            services.Configure<OpenIdConnectOptions>(AzureADDefaults.OpenIdScheme, options =>
            {
                options.Authority += "/v2.0/";
                options.TokenValidationParameters.ValidateIssuer = true;
                options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
            });

            //services.AddScoped<IProfileService, TestProfileService>();
        }
    }
}
