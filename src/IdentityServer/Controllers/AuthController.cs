using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Controllers
{
    public class AuthController : Controller
    {
        public AuthController(UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        public IActionResult SetCulture(string culture, string returnUrl)
        {
            HttpContext.Response.Cookies.Append(
               CookieRequestCultureProvider.DefaultCookieName,
               CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
               new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
               );
            return Redirect(returnUrl);

        }
        public IActionResult Login(string returnUrl)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            //check the model is valid.
            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.PassWord, false, false);
            if (result.Succeeded)
            {
                return Redirect(model.ReturnUrl);
            }
            else if (result.IsLockedOut)
            {

            }
            return View(model);
        }
        public async Task<IActionResult> Register(string returnUrl)
        {
            return await Task.Run(() => View(new RegisterViewModel { ReturnUrl = returnUrl }));
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return await Task.Run(() => View(model));
            }
            var user = new IdentityUser(model.UserName);
            user.Email = model.Email;
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