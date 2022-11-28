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

            builder.Services.ConfigureDbConnection(builder.Configuration);
            builder.Services.ConfigureAuthentication();
            builder.Services.ConfigureServices();

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