using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Test.core.Services;
using Test.dataaccess.Data;
using Test.model.Users;
using Test.webapi.Data;
using Test.webapi.Filter;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace Test.webapi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // so our claims will not be translated
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            //services.AddAuthentication("Bearer")
            //    .AddIdentityServerAuthentication(options =>
            //    {
            //        options.Authority = "https://localhost:6001";
            //        options.RequireHttpsMetadata = true;
            //        options.ApiName = "bankApi";
            //    });

            services.AddDbContext<AuthDbContext>(options =>
               options.UseSqlServer(Configuration.GetConnectionString("AuthDb")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AuthDbContext>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.Authority = "https://localhost:6001";
                o.Audience = "bankApi";
                o.RequireHttpsMetadata = true;
                o.SaveToken = true;
            });

            services.AddDbContext<BankContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("BankDatabase"));
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("mypolicy", policy =>
                    policy.Requirements.Add(new MyRequirement(SystemRoles.Admin))
                );
                options.AddPolicy("RequiresAdmin", policy =>
                    {
                        policy.RequireAuthenticatedUser();
                        policy.RequireClaim(ClaimTypes.Role, SystemRoles.Admin);
                    }
                );
            });
            services.AddControllers();

            services.AddSingleton<IAuthorizationHandler, MyHandler>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IRegisterService, RegisterService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(options => options.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
