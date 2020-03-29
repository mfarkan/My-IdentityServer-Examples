using IdentityServer.Infrastructure;
using IdentityServer.Models;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace IdentityServer.Controllers
{
    // ToDo : buralara kesinlikle bakmak gerekiyor admin girişi gibi.
    public class AdminController : BaseAdminController
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ConfigurationDbContext configurationDbContext;
        public AdminController(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.PassWord, false, false);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}