using IdentityMicroService.Application.Dto;
using IdentityMicroService.Application.Services.Abstractions;
using IdentityMicroService.Domain.Entities.Enums;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityMicroService.Presentation.Controllers;

[Route("[controller]/[action]")]
public class AuthController : Controller
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IAccountService _accountService;
    private readonly IEmailService _emailService;

    public AuthController(IAuthenticationService authenticationService, IAccountService accountService, IEmailService emailService)
    {
        _authenticationService = authenticationService;
        _accountService = accountService;
        _emailService = emailService;
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] UserModelForAuthorizationDto model)
    {
        if (!ModelState.IsValid) return UnprocessableEntity();

        var user = await _authenticationService.IsUserExistsAsync(model);
        if (user == null)
            return BadRequest("Either an email or a password is incorrect.");

        if (!await _emailService.IsEmailConfirmedAsync(user))
            return BadRequest("Your email has not been confirmed.");

        if (!await _authenticationService.UserSingInAsync(user, model))
            return BadRequest("Either an email or a password is incorrect.");

        var (accessToken, refreshToken) = await _authenticationService.GetTokensAsync(model);

        return Ok(new { accessToken, refreshToken });
    }

    [HttpPost]
    public async Task<IActionResult> SignUp([FromBody] RegistrationUserDto model)
    {
        if (!ModelState.IsValid) return UnprocessableEntity();

        if (await _emailService.CheckExistsEmail(model.Email))
        {
            return BadRequest("This email is already in use.");
        }

        var result = await _accountService.CreatePatientAsync(model);

        if (!result) return BadRequest("Can't create user, try again later.");

        var emailResult = await _emailService.SendEmailConfirmAsync(model);

        if (!emailResult) return BadRequest("Can't send mail in you email.");

        return Ok();
    }

    [Authorize(Roles = nameof(UserRole.Receptionist))]
    [HttpPut("{userId}")]
    public async Task<IActionResult> ChangeRole(Guid userId, RoleDto model)
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
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string token, [FromQuery] string email)
    {
        var result = await _emailService.ConfirmEmailAsync(email, token);
        return result ? Ok() : BadRequest("Failure Confirm Email");
    }

    [HttpGet]
    public async Task<IActionResult> IsEmailExists(string email)
    {
        return Json(await _emailService.CheckExistsEmail(email));
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(UserRole.Receptionist))]
    [HttpPost]
    public async Task<IActionResult> Doctor([FromBody] DoctorRegistrationDto model)
    {
        if (!ModelState.IsValid) return BadRequest("Model is wrong.");

        if (await _emailService.CheckExistsEmail(model.Email))
        {
            ModelState.AddModelError("Email", "This email is already in use.");
            return BadRequest("Model is wrong.");
        }

        var result = await _accountService.CreateDoctorAsync(model);

        if (result == null) return BadRequest("Something went wrong.");

        var emailResult = await _emailService.SendEmailConfirmForDoctorAsync(result);
        if (!emailResult)
            return BadRequest("The email was not sent.");

        return Created("", result.Id);
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(UserRole.Patient))]
    [HttpPut("{id}")]
    public async Task<IActionResult> PhotoChange(Guid id, [FromBody] Guid photoId)
    {
        await _accountService.ChangePhotoAsync(id, photoId);
        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> Doctors()
    {
        return Ok(await _accountService.GetAccountsByRole(UserRole.Doctor));
    }
}