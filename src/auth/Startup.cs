using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Test.auth.Data;

namespace Test.auth
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
               options.UseSqlServer(
                   Configuration.GetConnectionString("Default")));

            services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
               .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddIdentityServer(options =>
            {
                options.UserInteraction.LoginUrl = "/Account/Login";
                options.UserInteraction.LogoutUrl = "/Account/Logout";
            })
                .AddDeveloperSigningCredential()
                .AddInMemoryClients(Config.GetClients())
                .AddInMemoryApiResources(Config.GetApiResources())
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                //.AddApiAuthorization<ApplicationUser, ApplicationDbContext>()
                .AddAspNetIdentity<ApplicationUser>();

            //{
            //    options.Clients.AddSPA("myspa", spa =>
            //    {
            //        spa.
            //        spa.WithRedirectUri("");
            //        spa.WithLogoutRedirectUri("");
            //    });
            //});

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseCors("AllowAll");
            app.UseRouting();
            
            
            app.UseIdentityServer();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
