using IdentityMicroService.Application.Services.Abstractions;
using IdentityMicroService.Domain.Entities.Enums;
using IdentityMicroService.Domain.Entities.Models.AuthorizationDTO;
using IdentityServer4.Extensions;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityMicroService.Presentation.Controllers
{
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
            return View(new UserModelForAuthorizationDTO { returnUrl = returnUrl });
        }

        [HttpPost]
        public async Task<IActionResult> LoginAsync(UserModelForAuthorizationDTO model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await _authenticationService.ReturnUserIfValidAsync(model);

            if (user != null)
            {
                var (accessToken, refreshToken) = await _authenticationService.GetTokensAsync(model);

                if (model.returnUrl.IsNullOrEmpty())
                    return RedirectToAction(nameof(Index)); //TODO
                else
                    return Redirect(model.returnUrl);
            }

            ModelState.AddModelError("", "Either an email or a password is incorrect");

            return View();
        }

        [HttpGet]
        public IActionResult SingUp(string returnUrl)
        {
            return View(new RegistrationUserDTO { returnUrl = returnUrl });
        }

        [HttpPost]
        public async Task<IActionResult> SingUpAsync(RegistrationUserDTO model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await _authenticationService.CreateUserAsync(model);

            if (user != null)
            {
                await _authenticationService.AddUserRoleAsync(user, UserRole.Patient);
                var (accessToken, refreshToken) = await _authenticationService.GetTokensAsync(model);

                if (model.returnUrl.IsNullOrEmpty())
                    return RedirectToAction(nameof(Index)); //TODO
                else
                    return Redirect(model.returnUrl);
            }

            return View();
        }

        [Authorize(Roles = "Receptionist")]
        [HttpPut]
        public async Task<IActionResult> ChangeRoleAsync(string userId, string role)
        {
            var user = await _authenticationService.GetUserById(userId);
            if (Enum.TryParse(role, out UserRole roleEnum) && user != null)
            {
                await _authenticationService.AddUserRoleAsync(user, roleEnum);
                return Ok();
            }
            else return BadRequest();
        }

        [HttpGet]
        public async Task<IActionResult> SingOutAsync(string returnUrl)
        {
            await _authenticationService.SignOutAsync();
            return RedirectToAction(nameof(Index)); //TODO
        }

        /// <summary>
        /// ONLY FOR TESTS
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
