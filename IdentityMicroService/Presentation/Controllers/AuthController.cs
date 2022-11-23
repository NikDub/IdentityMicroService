using IdentityMicroService.Domain.Contracts;
using IdentityMicroService.Domain.Entities.Models;
using IdentityMicroService.Domain.Entities.Models.AuthorizationDTO;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

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

        /// <summary>
        /// ONLY FOR TESTS
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}
