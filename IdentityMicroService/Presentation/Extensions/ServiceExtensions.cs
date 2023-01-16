using IdentityMicroService.Application.Services;
using IdentityMicroService.Application.Services.Abstractions;
using IdentityMicroService.Domain.Entities.Models;
using IdentityMicroService.Infrastructure;
using IdentityMicroService.Presentation.IdentityConfiguration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace IdentityMicroService.Presentation.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Authority = configuration.GetValue<string>("Routes:AuthorityRoute") ??
                                    throw new NotImplementedException();
                options.Audience = configuration.GetValue<string>("Routes:Scopes") ??
                                   throw new NotImplementedException();
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidAudience = configuration.GetValue<string>("Routes:Scopes") ??
                                    throw new NotImplementedException(),
                    ValidateIssuer = true,
                    ValidIssuer = configuration.GetValue<string>("Routes:AuthorityRoute") ??
                                  throw new NotImplementedException(),
                    ValidateLifetime = true
                };
            });
    }

    public static void ConfigureAuthentication(this IServiceCollection services)
    {
        services.AddIdentity<Account, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddIdentityServer(options => { options.UserInteraction.LoginUrl = PathConfiguration.LoginPath; })
            .AddAspNetIdentity<Account>()
            .AddInMemoryIdentityResources(Configuration.GetIdentityResources())
            .AddInMemoryApiScopes(Configuration.GetApiScopes())
            .AddInMemoryApiResources(Configuration.GetApiResources())
            .AddInMemoryClients(Configuration.GetClients())
            .AddDeveloperSigningCredential()
            .AddExtensionGrantValidator<ResourceOwnerEmailPasswordExtensionGrantValidator>()
            .AddProfileService<ProfileService>();


        services.ConfigureApplicationCookie(config =>
        {
            config.Cookie.Name = PathConfiguration.CookieName;
            config.LoginPath = PathConfiguration.LoginPath;
        });
    }

    public static void ConfigureDbConnection(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(config =>
        {
            config.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("IdentityMicroService"));
        });
    }

    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IEmailService, EmailService>();

        services.Configure<RazorViewEngineOptions>(o =>
        {
            o.ViewLocationFormats.Clear();
            o.ViewLocationFormats.Add(PathConfiguration.PathToView + RazorViewEngine.ViewExtension);
        });
    }
}