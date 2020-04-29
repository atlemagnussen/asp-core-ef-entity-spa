using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Test.auth.Extentions;
using Test.auth.Services;
using Test.core.Services;
using Test.dataaccess;

namespace Test.auth
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; set; }

        public Startup(IConfiguration configuration,
            IWebHostEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // cookie policy to deal with temporary browser incompatibilities
            services.AddSameSiteCookiePolicy();

            var configAzAd = Configuration.GetSection("AzureAd");
            services.Configure<SettingsAzureAd>(configAzAd);
            services.Configure<SettingsAzureKeyVault>(Configuration.GetSection("AzureKeyVault"));
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<ILogoutService, LogoutService>();
            services.AddScoped<IRegisterService, RegisterService>();
            services.AddScoped<IClaimsHelper, ClaimsHelper>();
            services.AddScoped<IExternalService, ExternalService>();
            services.AddScoped<IAzureKeyService, AzureKeyService>();
            
            services.AddScoped<ITokenCreationService, AzureKeyVaultTokenCreationService>();
            services.AddScoped<ISigningCredentialStore, AzureSigningCredentialsStore>();
            services.AddScoped<IValidationKeysStore, AzureValidationKeysStore>();
            services.AddIdentityServerConfig(Configuration, Environment, configAzAd);
            services.AddCommonIdentitySettings();
            services.AddCommonDataProtection();

            services.AddRazorPages();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
            //app.UseDeveloperExceptionPage();
            // app.UseDatabaseErrorPage();
            
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
            app.UseHttpsRedirection();
            
            app.UseStaticFiles();
            app.UseCors("AllowAll");
            app.UseRouting();
            
            
            app.UseIdentityServer();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
