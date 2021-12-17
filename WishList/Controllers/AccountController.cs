using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WishList.Models;
using WishList.Models.AccountViewModels;

namespace WishList.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult Login([FromForm] LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var signInResult = _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            if (!signInResult.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "invalid login attempt");
                return View(model);
            }

            return RedirectToAction("Index", "Item");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            _signInManager.SignOutAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            return RedirectToAction("Index", "Home");
        }



        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Register([FromForm] RegisterViewModel model)
        {

            if (!ModelState.IsValid)
                return View(model);

            var createResult = _userManager.CreateAsync(new ApplicationUser() { UserName = model.Email, Email = model.Email }, model.Password)
                .ConfigureAwait(false).GetAwaiter().GetResult();
            if (!createResult.Succeeded)
            {
                foreach (var error in createResult.Errors)
                {
                    ModelState.AddModelError("Password", error.Description);
                }

                return View(model);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
