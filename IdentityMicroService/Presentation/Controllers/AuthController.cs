using IdentityMicroService.Application.Services.Abstractions;
using IdentityMicroService.Application.Services.AuthorizationDTO;
using IdentityMicroService.Domain.Entities.Enums;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityMicroService.Presentation.Controllers;

[Route("[controller]/[action]")]
public class AuthController : Controller
{
    private readonly IAuthenticationService _authenticationService;

    public AuthController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpGet]
    public IActionResult Login(string returnUrl)
    {
        return View(new UserModelForAuthorizationDto { ReturnUrl = returnUrl });
    }

    [HttpPost]
    public async Task<IActionResult> Login(UserModelForAuthorizationDto model)
    {
        if (!ModelState.IsValid) return View();

        var user = await _authenticationService.ReturnUserIfValidAsync(model);

        if (user == null)
        {
            ModelState.AddModelError("", "Either an email or a password is incorrect");

            return View();
        }

        var (accessToken, refreshToken) = await _authenticationService.GetTokensAsync(model);

        if (model.ReturnUrl.IsNullOrEmpty())
            return Redirect("/");
        return Redirect(model.ReturnUrl);
    }

    [HttpGet]
    public IActionResult SignUp(string returnUrl)
    {
        return View(new RegistrationUserDto { ReturnUrl = returnUrl });
    }

    [HttpPost]
    public async Task<IActionResult> SignUp(RegistrationUserDto model)
    {
        if (!ModelState.IsValid) return View();

        if (await _authenticationService.CheckExistsEmail(model.Email))
        {
            ModelState.AddModelError("Email", "This email is already in use");
            return View();
        }

        var result = await _authenticationService.CreatePatientAsync(model);

        if (!result) return View();

        var emailResult = await _authenticationService.SendEmailConfirmAsync(model, Url);
        var (accessToken, refreshToken) = await _authenticationService.GetTokensAsync(model);

        if (!emailResult) return BadRequest(model.ReturnUrl);

        if (model.ReturnUrl.IsNullOrEmpty())
            return Redirect("/");
        return Redirect(model.ReturnUrl);
    }

    [Authorize(Roles = nameof(UserRole.Receptionist))]
    [HttpPut("{userId}")]
    public async Task<IActionResult> ChangeRole(string userId, RoleDto model)
    {
        var user = await _authenticationService.GetUserById(userId);
        if (Enum.TryParse(model.Role, out UserRole roleEnum) && user != null)
        {
            await _authenticationService.AddUserRoleAsync(user, roleEnum);
            return Ok();
        }

        return BadRequest();
    }

    [HttpGet]
    public async Task<IActionResult> SingOut()
    {
        await _authenticationService.SignOutAsync();
        return Redirect("/");
    }

    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(string token, string email)
    {
        var result = await _authenticationService.ConfirmEmailAsync(email, token);
        return View(result ? "SuccessConfirmEmail " : "FailureConfirmEmail");
    }

    [HttpGet]
    public async Task<IActionResult> IsEmailExists(string email)
    {
        return Json(await _authenticationService.CheckExistsEmail(email));
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(UserRole.Receptionist))]
    [HttpPost]
    public async Task<IActionResult> Doctor([FromBody] DoctorRegistrationDto model)
    {
        if (!ModelState.IsValid) return BadRequest("Model is wrong.");

        if (await _authenticationService.CheckExistsEmail(model.Email))
        {
            ModelState.AddModelError("Email", "This email is already in use.");
            return BadRequest("Model is wrong.");
        }

        var result = await _authenticationService.CreateDoctorAsync(model);

        if (result == null) return BadRequest("Something went wrong.");

        var emailResult = await _authenticationService.SendEmailConfirmForDoctorAsync(result, Url);
        if (!emailResult)
            return BadRequest("The email was not sent.");

        return Created("", result.Id);
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(UserRole.Patient))]
    [HttpPut("{id}")]
    public async Task<IActionResult> PhotoChange(string id, [FromBody] string photoId)
    {
        await _authenticationService.ChangePhotoAsync(id, photoId);
        return NoContent();
    }
}