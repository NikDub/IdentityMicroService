using IdentityMicroService.Application.Services.Abstractions;
using IdentityMicroService.Domain.Entities.Models;
using IdentityMicroService.Domain.Entities.Models.AuthorizationDTO;
using IdentityMicroService.Presentation.Extensions;
using IdentityServer4;
using Microsoft.AspNetCore.Identity;

namespace IdentityMicroService.Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<Account> _userManager;
        private readonly SignInManager<Account> _signInManager;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public AuthenticationService(UserManager<Account> userManager, SignInManager<Account> signInManager, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<(string accessToken, string refreshToken)> GetTokensAsync(UserModelForAuthorizationDTO userForAuthentication)
        {
            var client = _httpClientFactory.CreateClient();
            EmailPasswordTokenRequest tokenRequest = new EmailPasswordTokenRequest()
            {
                Address = PathConfiguration.tokenRoute,
                GrantType = PathConfiguration.resourceOwnerEmailPassword,
                ClientId = PathConfiguration.clientId,
                Scope = $"{PathConfiguration.testApiScope} {IdentityServerConstants.StandardScopes.OpenId} {IdentityServerConstants.StandardScopes.OfflineAccess}",
                Email = userForAuthentication.email,
                Password = userForAuthentication.password,
            };
            var tokenResponse = await client.RequestEmailPasswordTokenAsync(tokenRequest);
            return (tokenResponse.AccessToken, tokenResponse.RefreshToken);
        }


        public async Task<Account> ReturnUserIfValidAsync(UserModelForAuthorizationDTO userForAuthentication)
        {
            var user = await _userManager.FindByEmailAsync(userForAuthentication.email);

            var res = await _signInManager.PasswordSignInAsync(user, userForAuthentication.password, false, false);

            if (res.Succeeded)
            {
                return user;
            }
            return null;
        }

        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<Account> CreateUserAsync(RegistrationUserDTO model)
        {
            var result = await _userManager.CreateAsync(new Account { Email = model.email, UserName = model.email }, model.password);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.email);
                if (user != null)
                {
                    var res = await _signInManager.PasswordSignInAsync(user, model.password, false, false);

                    if (res.Succeeded)
                    {
                        return user;
                    }
                }
            }
            return null;
        }
    }
}
