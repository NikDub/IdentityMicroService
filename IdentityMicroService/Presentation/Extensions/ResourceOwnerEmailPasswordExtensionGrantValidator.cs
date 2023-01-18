using IdentityMicroService.Domain.Entities.Models;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using static IdentityModel.OidcConstants;

namespace IdentityMicroService.Presentation.Extensions;

public class ResourceOwnerEmailPasswordExtensionGrantValidator : IExtensionGrantValidator
{
    private readonly SignInManager<Account> _signInManager;
    private readonly UserManager<Account> _userManager;

    public ResourceOwnerEmailPasswordExtensionGrantValidator(UserManager<Account> userManager,
        SignInManager<Account> signInManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    public string GrantType => PathConfiguration.ResourceOwnerEmailPassword;

    public async Task ValidateAsync(ExtensionGrantValidationContext context)
    {
        var email = context.Request.Raw.Get(StandardScopes.Email);
        var password = context.Request.Raw.Get(TokenRequest.Password);

        if (email != null && password != null)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                var result = await _signInManager.CheckPasswordSignInAsync(user, password, true);
                if (result.Succeeded)
                {
                    var sub = await _userManager.GetUserIdAsync(user);

                    context.Result = new GrantValidationResult(sub, AuthenticationMethods.Password);
                    return;
                }
            }
        }


        context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
    }
}