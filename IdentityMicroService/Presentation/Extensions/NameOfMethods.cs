using IdentityMicroService.Presentation.Controllers;

namespace IdentityMicroService.Presentation.Extensions;

public static class NameOfMethods
{
    public const string AuthActionConfirmEmail = nameof(AuthController.ConfirmEmail);
    public static string AuthControllerName = nameof(AuthController).Replace("Controller", "");
}