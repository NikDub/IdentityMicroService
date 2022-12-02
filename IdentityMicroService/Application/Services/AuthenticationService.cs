using IdentityMicroService.Application.Services.Abstractions;
using IdentityMicroService.Domain.Entities.Enums;
using IdentityMicroService.Domain.Entities.Models;
using IdentityMicroService.Domain.Entities.Models.AuthorizationDTO;
using IdentityMicroService.Infrastructure;
using IdentityMicroService.Presentation.Extensions;
using IdentityServer4;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityMicroService.Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<Account> _userManager;
        private readonly SignInManager<Account> _signInManager;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDBContext _dBContext;
        private readonly IEmailService _emailService;

        public AuthenticationService(UserManager<Account> userManager,
            SignInManager<Account> signInManager,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ApplicationDBContext dBContext,
            IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _dBContext = dBContext;
            _emailService = emailService;
        }

        public async Task<(string accessToken, string refreshToken)> GetTokensAsync(UserModelForAuthorizationDTO userForAuthentication)
        {
            var webUrl = _configuration.GetValue<string>("Endpoints:Https:Url");
            var client = _httpClientFactory.CreateClient();
            EmailPasswordTokenRequest tokenRequest = new EmailPasswordTokenRequest()
            {
                Address = $"{webUrl}{PathConfiguration.TokenRoute}",
                GrantType = PathConfiguration.ResourceOwnerEmailPassword,
                ClientId = PathConfiguration.ClientId,
                Scope = $"{PathConfiguration.TestApiScope} {IdentityServerConstants.StandardScopes.OpenId} {IdentityServerConstants.StandardScopes.OfflineAccess}",
                Email = userForAuthentication.Email,
                Password = userForAuthentication.Password,
            };
            var tokenResponse = await client.RequestEmailPasswordTokenAsync(tokenRequest);
            return (tokenResponse.AccessToken, tokenResponse.RefreshToken);
        }


        public async Task<Account> ReturnUserIfValidAsync(UserModelForAuthorizationDTO userForAuthentication)
        {
            var user = await _userManager.FindByEmailAsync(userForAuthentication.Email);

            if (user != null)
            {
                var res = await _signInManager.PasswordSignInAsync(user, userForAuthentication.Password, false, false);

                if (res.Succeeded)
                {
                    return user;
                }
            }
            return null;
        }

        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<Account> CreateUserAsync(RegistrationUserDTO model)
        {
            var result = await _userManager.CreateAsync(new Account { Email = model.Email, UserName = model.Email }, model.Password);
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

        public async Task AddUserRoleAsync(Account user, UserRole role)
        {
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Count > 0)
            {
                await _userManager.RemoveFromRolesAsync(user, roles);
            }

            await _userManager.AddToRoleAsync(user, role.ToString());
        }

        public async Task<Account> GetUserById(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<bool> CreatePatientAsync(RegistrationUserDTO model)
        {
            var transaction = await _dBContext.Database.BeginTransactionAsync();

            try
            {
                var user = await CreateUserAsync(model);

                await AddUserRoleAsync(user, UserRole.Patient);

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> SendEmailConfirmAsync(RegistrationUserDTO model, IUrlHelper url)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if(user != null)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                if (token != null)
                {
                    var confirmationLink = url.Action("ConfirmEmail", "Auth", new { token, email = model.Email }, "https");
                    if (confirmationLink != null)
                    {
                        bool emailResponse = _emailService.SendEmail(model.Email, confirmationLink);
                        return emailResponse;
                    }
                }
            }
            return false;

        }

        public async Task<bool> ConfirmEmailAsync(string email, string token)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if(user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if(result.Succeeded)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> CheckExistsEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user != null;
        } 
    }
}
