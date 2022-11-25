using IdentityMicroService.Application.Services.Abstractions;
using IdentityMicroService.Domain.Entities.Models.AuthorizationDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityMicroService.Presentation.Controllers
{
    [Route("[controller]/[action]")]
    public class AuthController : Controller
    {
        private readonly IAuthenticationService _authenticationManager;

        public AuthController(IAuthenticationService authenticationManager)
        {
            _authenticationManager = authenticationManager;
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

            var user = await _authenticationManager.ReturnUserIfValidAsync(model);

            if (user != null)
            {
                var (accessToken, refreshToken) = await _authenticationManager.GetTokensAsync(model);
                return Redirect(model.returnUrl);
            }

            ModelState.AddModelError("", "Something went wrong");

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

            var user = await _authenticationManager.CreateUserAsync(model);

            if (user != null)
            {
                var (accessToken, refreshToken) = await _authenticationManager.GetTokensAsync(model);
                return Redirect(model.returnUrl);
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> SingOutAsync(string returnUrl)
        {
            await _authenticationManager.SignOutAsync();
            return Redirect("/Auth/Index");
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
