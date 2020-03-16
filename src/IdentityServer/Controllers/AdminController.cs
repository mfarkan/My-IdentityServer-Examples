using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Controllers
{
    // TODO : this controller must be authorize by SuperAdmin policy!
    //[Authorize]
    public class AdminController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        public AdminController(UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {

            }
            return View();
        }
        [AllowAnonymous]
        public async Task<IActionResult> Login()
        {
            return await Task.Run(() => View(new LoginViewModel()));
        }
        [Authorize(Policy = "SuperAdmin")]
        public IActionResult Index()
        {
            return View();
        }
    }
}