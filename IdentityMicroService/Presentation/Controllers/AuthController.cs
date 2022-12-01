﻿using IdentityMicroService.Application.Services.Abstractions;
using IdentityMicroService.Domain.Entities.Enums;
using IdentityMicroService.Domain.Entities.Models.AuthorizationDTO;
using IdentityServer4.Extensions;
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
            return View(new UserModelForAuthorizationDTO { ReturnUrl = returnUrl });
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserModelForAuthorizationDTO model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await _authenticationService.ReturnUserIfValidAsync(model);

            if (user != null)
            {
                var (accessToken, refreshToken) = await _authenticationService.GetTokensAsync(model);

                if (model.ReturnUrl.IsNullOrEmpty())
                    return RedirectToAction(nameof(Index)); //TODO
                else
                    return Redirect(model.ReturnUrl);
            }

            ModelState.AddModelError("", "Either an email or a password is incorrect");

            return View();
        }

        [HttpGet]
        public IActionResult SignUp(string returnUrl)
        {
            return View(new RegistrationUserDTO { ReturnUrl = returnUrl });
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(RegistrationUserDTO model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var result = await _authenticationService.CreatePatientAsync(model);

            if (result != null)
            {
                var (accessToken, refreshToken) = await _authenticationService.GetTokensAsync(model);

                if (model.ReturnUrl.IsNullOrEmpty())
                    return RedirectToAction(nameof(Index)); //TODO
                else
                    return Redirect(model.ReturnUrl);
            }

            return View();
        }

        [Authorize(Roles = nameof(UserRole.Receptionist))]
        [HttpPut]
        public async Task<IActionResult> ChangeRole(string userId, string role)
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
        public async Task<IActionResult> SingOut(string returnUrl)
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
