using IdentityMicroService.Application.Services.Abstractions;
using IdentityMicroService.Application.Services.AuthorizationDTO;
using IdentityMicroService.Domain.Entities.Enums;
using IdentityMicroService.Domain.Entities.Models;
using IdentityMicroService.Infrastructure;
using IdentityMicroService.Presentation.Extensions;
using IdentityServer4;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityMicroService.Application.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _dBContext;
    private readonly IEmailService _emailService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly SignInManager<Account> _signInManager;
    private readonly UserManager<Account> _userManager;

    public AuthenticationService(UserManager<Account> userManager,
        SignInManager<Account> signInManager,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ApplicationDbContext dBContext,
        IEmailService emailService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _dBContext = dBContext;
        _emailService = emailService;
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

    public async Task<Account> ReturnUserIfValidAsync(UserModelForAuthorizationDto userForAuthentication)
    {
        var user = await _userManager.FindByEmailAsync(userForAuthentication.Email);
        if (user == null) return null;

        var res = await _signInManager.PasswordSignInAsync(user, userForAuthentication.Password, false, false);
        if (res.Succeeded) return user;
        return null;
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

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null) return null;

        var res = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
        if (res.Succeeded) return user;

        return null;
    }

    public async Task AddUserRoleAsync(Account user, UserRole role)
    {
        var roles = await _userManager.GetRolesAsync(user);
        if (roles.Count > 0) await _userManager.RemoveFromRolesAsync(user, roles);

        await _userManager.AddToRoleAsync(user, role.ToString());
    }

    public async Task<Account> GetUserById(string userId)
    {
        return await _userManager.FindByIdAsync(userId);
    }

    public async Task<bool> CreatePatientAsync(RegistrationUserDto model)
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

    public async Task<bool> SendEmailConfirmAsync(RegistrationUserDto model, IUrlHelper url)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user != null) return false;

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        if (token.IsNullOrEmpty()) return false;

        var confirmationLink = url.Action(NameOfMethods.AuthActionConfirmEmail, NameOfMethods.AuthControllerName,
            new { token, email = model.Email }, PathConfiguration.Https);
        if (confirmationLink == null) return false;

        var emailResponse = _emailService.SendEmail(model.Email, confirmationLink);
        return emailResponse;
    }

    public async Task<bool> ConfirmEmailAsync(string email, string token)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return false;

        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (result.Succeeded) return true;

        return false;
    }

    public async Task<bool> CheckExistsEmail(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user != null;
    }

    public async Task<Account> CreateDoctorAsync(DoctorRegistrationDto model)
    {
        var transaction = await _dBContext.Database.BeginTransactionAsync();
        Account user;
        try
        {
            user = await CreateUserDoctorAsync(model);

            await AddUserRoleAsync(user, UserRole.Doctor);

            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            return null;
        }

        return user;
    }

    public async Task<Account> CreateUserDoctorAsync(DoctorRegistrationDto model)
    {
        var password = Guid.NewGuid().ToString("N").ToLower()
            .Replace("1", "").Replace("o", "").Replace("0", "")
            .Substring(0, 10);
        var result =
            await _userManager.CreateAsync(
                new Account { Email = model.Email, UserName = model.Email, PhotoId = model.PhotoId }, password);
        if (!result.Succeeded) return null;

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null) return null;


        var res = await _signInManager.PasswordSignInAsync(user, password, false, false);
        user.PasswordHash = password;
        if (res.Succeeded) return user;

        return null;
    }

    public async Task<bool> SendEmailConfirmForDoctorAsync(Account model, IUrlHelper url)
    {
        if (model == null)
            return false;

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null) return false;

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        if (token.IsNullOrEmpty()) return false;

        var confirmationLink = url.Action(NameOfMethods.AuthActionConfirmEmail, NameOfMethods.AuthControllerName,
            new { token, email = model.Email }, PathConfiguration.Https);
        if (confirmationLink == null) return false;

        confirmationLink = $"Username: {model.UserName}\n" +
                                       $"Password: {model.PasswordHash}\n" +
                                       $"{confirmationLink}";

        var emailResponse = _emailService.SendEmail(model.Email, confirmationLink);
        return emailResponse;
    }

    public async Task ChangePhotoAsync(string userId, string photoId)
    {
        var user = await GetUserById(userId);
        user.PhotoId = photoId;
        await _userManager.UpdateAsync(user);
    }
}