using IdentityMicroService.Domain.Contracts;
using IdentityMicroService.Domain.Entities.Models.AuthorizationDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace IdentityMicroService.Presentation.Controllers
{
    [Route("[controller]")]
    public class AuthController : Controller
    {
        public IAuthenticationManage _authenticationManage;

        public AuthController(IAuthenticationManage authenticationManage)
        {
            _authenticationManage = authenticationManage;
        }

        [Route("[action]")]
        public IActionResult Login(string ReturnUrl)
        {
            return View(new UserModelForAuthorizationDTO { ReturnUrl = ReturnUrl });
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> LoginAsync(UserModelForAuthorizationDTO model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await _authenticationManage.ReturnUserIfValid(model);

            if (user != null)
            {
                var (accessToken, refreshToken) = await _authenticationManage.GetTokensAsync(model);
                return Redirect(model.ReturnUrl);
            }

            ModelState.AddModelError("", "Something went wrong");

            return View();
        }


        [Route("[action]")]
        public IActionResult SingUp(string ReturnUrl)
        {
            return View(new RegistrationUserDTO { ReturnUrl = ReturnUrl });
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> SingUp(RegistrationUserDTO model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await _authenticationManage.CreateUser(model);

            if (user != null)
            {
                var (accessToken, refreshToken) = await _authenticationManage.GetTokensAsync(model);
                return Redirect(model.ReturnUrl);
            }

            return View();
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> SingOut(string ReturnUrl)
        {
            await _authenticationManage.SignOut();
            return Redirect("/Auth/Index");
        }

        /// <summary>
        /// ONLY FOR TESTS
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
