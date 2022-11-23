using IdentityMicroService.Domain.Contracts;
using IdentityMicroService.Domain.Entities.Models;
using IdentityMicroService.Domain.Entities.Models.AuthorizationDTO;
using IdentityModel.Client;
using Microsoft.AspNetCore.Identity;

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
            PasswordTokenRequest tokenRequest = new PasswordTokenRequest()
            {
                Address = "https://localhost:8080/connect/token",
                ClientId = "myClient",
                Scope = "TestsAPI offline_access openid",
                UserName = userForAuthentication.UserName,
                Password = userForAuthentication.Password

            };
            var tokenResponse = await client.RequestPasswordTokenAsync(tokenRequest);
            return (tokenResponse.AccessToken, tokenResponse.RefreshToken);
        }

        public async Task<Accounts> ReturnUserIfValid(UserModelForAuthorizationDTO userForAuthentication)
        {
            var user = await _userManager.FindByNameAsync(userForAuthentication.UserName);

            var res = await _signInManager.PasswordSignInAsync(userForAuthentication.UserName, userForAuthentication.Password, false, false);

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
    }
}
