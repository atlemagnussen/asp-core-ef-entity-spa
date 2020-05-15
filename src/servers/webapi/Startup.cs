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
using Test.model.Users;
using Test.webapi.Filter;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Logging;
using IdentityModel;
using Test.dataaccess;

namespace Test.webapi
{
    public class Startup
    {
        public Startup(IConfiguration configuration,
            IWebHostEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // so our claims will not be translated
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            IdentityModelEventSource.ShowPII = true;

            var authConStr = Configuration.GetConnectionString("AuthDb");
            services.AddDbContext<DataProtectionDbContext>(options =>
                options.UseSqlServer(authConStr));

            services.AddDbContext<AuthDbContext>(options =>
               options.UseSqlServer(authConStr));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AuthDbContext>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.Authority = Configuration.GetValue<string>("AuthServerUrl");
                o.Audience = "bankApi";
                o.RequireHttpsMetadata = true;
                o.SaveToken = true;
            });

            services.AddCommonIdentitySettings();
            services.AddCommonDataProtection(Configuration.GetSection("AzureKeyVault"), Environment.IsDevelopment());

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
                        policy.RequireClaim(JwtClaimTypes.Role, SystemRoles.Admin);
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
