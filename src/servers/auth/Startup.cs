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
            var configAzKv = Configuration.GetSection("AzureKeyVault");
            services.Configure<SettingsAzureAd>(configAzAd);
            services.Configure<SettingsAzureKeyVault>(configAzKv);
            services.AddTransient<ILoginService, LoginService>();
            services.AddTransient<ILogoutService, LogoutService>();
            services.AddTransient<IRegisterService, RegisterService>();
            services.AddTransient<IClaimsHelper, ClaimsHelper>();
            services.AddTransient<IExternalService, ExternalService>();
            services.AddTransient<IAzureKeyService, AzureKeyService>();
            services.AddTransient<IAdminService, AdminService>();
            
            // services that override default identityserver services
            services.AddTransient<ITokenCreationService, AzureKeyVaultTokenCreationService>();
            services.AddTransient<ISigningCredentialStore, AzureSigningCredentialsStore>();
            services.AddTransient<IValidationKeysStore, AzureValidationKeysStore>();

            services.AddIdentityServerConfig(Configuration, Environment, configAzAd);
            services.AddCommonIdentitySettings();
            services.AddCommonDataProtection(configAzKv, Environment.IsDevelopment());

            services.AddRazorPages();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (Environment.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseExceptionHandler("/Error");

            // app.UseDatabaseErrorPage();

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
