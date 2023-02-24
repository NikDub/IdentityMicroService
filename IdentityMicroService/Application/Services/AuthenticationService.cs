using IdentityMicroService.Application.Dto;
using IdentityMicroService.Application.Services.Abstractions;
using IdentityMicroService.Domain.Entities.Enums;
using IdentityMicroService.Domain.Entities.Models;
using IdentityMicroService.Presentation.Extensions;
using IdentityServer4;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

namespace IdentityMicroService.Application.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly SignInManager<Account> _signInManager;
    private readonly UserManager<Account> _userManager;

    public AuthenticationService(UserManager<Account> userManager,
        SignInManager<Account> signInManager,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<(string accessToken, string refreshToken)> GetTokensAsync(
        UserModelForAuthorizationDto userForAuthentication)
    {
        var webUrl = _configuration.GetValue<string>("Endpoints:Https:Url");
        var client = _httpClientFactory.CreateClient();
        var tokenRequest = new EmailPasswordTokenRequest
        {
            Address = $"{webUrl}{PathConfiguration.TokenRoute}",
            GrantType = PathConfiguration.ResourceOwnerEmailPassword,
            ClientId = PathConfiguration.ClientId,
            Scope =
                $"{PathConfiguration.TestApiScope} {IdentityServerConstants.StandardScopes.OpenId} {IdentityServerConstants.StandardScopes.OfflineAccess}",
            Email = userForAuthentication.Email,
            Password = userForAuthentication.Password
        };
        var tokenResponse = await client.RequestEmailPasswordTokenAsync(tokenRequest);
        return (tokenResponse.AccessToken, tokenResponse.RefreshToken);
    }

    public async Task<Account> IsUserExistsAsync(UserModelForAuthorizationDto userForAuthentication)
    {
        return await _userManager.FindByEmailAsync(userForAuthentication.Email);
    }

    public async Task<bool> UserSingInAsync(Account user, UserModelForAuthorizationDto userForAuthentication)
    {
        var res = await _signInManager.PasswordSignInAsync(user, userForAuthentication.Password, false, false);
        return res.Succeeded;
    }

    public async Task SignOutAsync()
    {
        await _signInManager.SignOutAsync();
    }
    public async Task<Account> CreateUserAsync(RegistrationUserDto model)
    {
        var result = await _userManager.CreateAsync(new Account { Email = model.Email, UserName = model.Email },
            model.Password);
        if (!result.Succeeded) return null;

        return await _userManager.FindByEmailAsync(model.Email);
    }
    public async Task AddUserRoleAsync(Account user, UserRole role)
    {
        var roles = await _userManager.GetRolesAsync(user);
        if (roles.Count > 0) await _userManager.RemoveFromRolesAsync(user, roles);

        await _userManager.AddToRoleAsync(user, role.ToString());
    }
    public async Task<Account> GetUserById(Guid userId)
    {
        return await _userManager.FindByIdAsync(userId.ToString());
    }
}