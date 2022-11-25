using IdentityMicroService.Domain.Contracts;
using IdentityMicroService.Domain.Entities.Models;
using IdentityMicroService.Infrastructure;
using IdentityMicroService.Infrastructure.Repository;
using IdentityMicroService.Presentation.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;

namespace IdentityMicroService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<ApplicationDBContext>(config =>
                {
                    config.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
                })
                .AddIdentity<Accounts, IdentityRole>(options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequiredLength = 6;
                })
                .AddEntityFrameworkStores<ApplicationDBContext>();

            builder.Services.ConfigureApplicationCookie(config =>
            {
                config.Cookie.Name = "IdentityServer.Cookies";
                config.LoginPath = "/Auth/Login";
            });
            builder.Services.AddScoped<IAuthenticationManage, AuthenticationManager>();

            builder.Services.AddIdentityServer(options =>
            {
                options.UserInteraction.LoginUrl = "/Auth/Login";
            })
                .AddAspNetIdentity<Accounts>()
                .AddInMemoryIdentityResources(Configuration.GetIdentityResources())
                .AddInMemoryApiScopes(Configuration.GetApiScopes())
                .AddInMemoryApiResources(Configuration.GetApiResources())
                .AddInMemoryClients(Configuration.GetClients())
                .AddDeveloperSigningCredential()
                .AddExtensionGrantValidator<ResourceOwnerEmailPasswordExtensionGrantValidator>();

            builder.Services.Configure<RazorViewEngineOptions>(o =>
            {
                o.ViewLocationFormats.Clear();
                o.ViewLocationFormats.Add("/Presentation/Views/{1}/{0}" + RazorViewEngine.ViewExtension);
            });

            builder.Services.AddControllersWithViews()
                .AddRazorRuntimeCompilation();
            builder.Services.AddEndpointsApiExplorer();

            var app = builder.Build();

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseIdentityServer();

            app.UseCookiePolicy();
            app.MapControllers();

            app.Run();
        }
    }
}