﻿using IdentityServer.Models;
using IdentityServer.Resources;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Threading.Tasks;

namespace IdentityServer.Controllers
{
    public class AuthController : Controller
    {
        public AuthController(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IIdentityServerInteractionService interactionService)
        {
            _interactionService = interactionService;
            _signInManager = signInManager;
            _userManager = userManager;
        }
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IIdentityServerInteractionService _interactionService;
        private readonly IStringLocalizer<SharedResource> _localizer;
        public IActionResult SetCulture(string culture, string returnUrl)
        {
            HttpContext.Response.Cookies.Append(
               CookieRequestCultureProvider.DefaultCookieName,
               CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
               new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
               );
            return Redirect(returnUrl);

        }
        public async Task<IActionResult> LogOut(string logOutId)
        {
            await _signInManager.SignOutAsync();
            var logOutRequest = await _interactionService.GetLogoutContextAsync(logOutId);
            if (string.IsNullOrEmpty(logOutRequest.PostLogoutRedirectUri))
            {
                return RedirectToAction("Index", "Home");
            }
            return Redirect(logOutRequest.PostLogoutRedirectUri);
        }
        public IActionResult Login(string returnUrl)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                ModelState.AddModelError("CheckYourLogin", _localizer["CheckYourLogin"]);
            }
            else
            {
                await _signInManager.SignOutAsync();
                //check the model is valid.
                var result = await _signInManager.PasswordSignInAsync(user, model.PassWord, false, false);
                if (result.Succeeded)
                {
                    var userIsGod = await _userManager.IsInRoleAsync(user, "CourierManageGodMode");
                    await _userManager.ResetAccessFailedCountAsync(user);
                    return Redirect(model.ReturnUrl);
                }
            }

            return View(model);
        }
        public IActionResult Register(string returnUrl)
        {
            return View(new RegisterViewModel { ReturnUrl = returnUrl });
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = new ApplicationUser(model.UserName)
            {
                Email = model.Email
            };
            var result = await _userManager.CreateAsync(user, model.PassWord);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);
                return Redirect(model.ReturnUrl);
            }
            return View();
        }
    }
}