using IdentityMicroService.Presentation.Extensions;

namespace IdentityMicroService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.ConfigureDbConnection(builder.Configuration);
        builder.Services.ConfigureJwtAuthentication(builder.Configuration);
        builder.Services.ConfigureAuthentication();
        builder.Services.ConfigureServices();

        builder.Services.AddControllersWithViews()
            .AddRazorRuntimeCompilation();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddCors();
        var app = builder.Build();
        app.UseCors(x => x.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseIdentityServer();

        app.MapControllers();

        app.Run();
    }
}