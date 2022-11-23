using IdentityMicroService.Domain.Contracts;
using IdentityMicroService.Domain.Entities.Models;
using IdentityMicroService.Domain.Entities.Models.AuthorizationDTO;
using IdentityMicroService.Presentation.Extensions;
using IdentityModel;
using IdentityModel.Client;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace IdentityMicroService.Infrastructure
{
    public class AuthenticationManager : IAuthenticationManage
    {
        public UserManager<Accounts> _userManager { get; }
        public SignInManager<Accounts> _signInManager { get; }
        public IHttpClientFactory _httpClientFactory { get; }

        public AuthenticationManager(UserManager<Accounts> userManager, SignInManager<Accounts> signInManager, IHttpClientFactory httpClientFactory)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<(string accessToken, string refreshToken)> GetTokensAsync(UserModelForAuthorizationDTO userForAuthentication)
        {
            var client = _httpClientFactory.CreateClient();
            EmailPasswordTokenRequest tokenRequest = new EmailPasswordTokenRequest()
            {
                Address = "https://localhost:8080/connect/token",
                GrantType = "emailpassword",
                ClientId = "myClient",
                Scope = "TestsAPI offline_access openid",
                Email = userForAuthentication.Email,
                Password = userForAuthentication.Password,
            };
            var tokenResponse = await client.RequestEmailPasswordTokenAsync(tokenRequest);
            return (tokenResponse.AccessToken, tokenResponse.RefreshToken);
        }


        public async Task<Accounts> ReturnUserIfValid(UserModelForAuthorizationDTO userForAuthentication)
        {
            var user = await _userManager.FindByEmailAsync(userForAuthentication.Email);

            var res = await _signInManager.PasswordSignInAsync(user, userForAuthentication.Password, false, false);

            if (res.Succeeded)
            {
                return user;
            }
            return null;
        }

        public async Task SignOut()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<Accounts> CreateUser(RegistrationUserDTO model)
        {
            var result = await _userManager.CreateAsync(new Accounts { Email = model.Email, UserName = model.Email }, model.Password);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null) 
                {
                    var res = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

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
