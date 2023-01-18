using System.Security.Claims;
using IdentityMicroService.Domain.Entities.Models;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;

namespace IdentityMicroService.Presentation.IdentityConfiguration;

public class ProfileService : IProfileService
{
    private readonly UserManager<Account> _userManager;

    public ProfileService(UserManager<Account> userManager)
    {
        _userManager = userManager;
    }

    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var user = await _userManager.GetUserAsync(context.Subject);
        if (user != null)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new("roles", userRoles.FirstOrDefault() ?? string.Empty)
            };

            context.IssuedClaims.AddRange(claims);
        }
    }

    public async Task IsActiveAsync(IsActiveContext context)
    {
        var user = await _userManager.GetUserAsync(context.Subject);

        context.IsActive = user != null;
    }
}