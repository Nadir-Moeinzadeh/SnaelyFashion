using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SnaelyFashion_AdminMVC.Services.IServices;
using SnaelyFashion_Models.DTO.ApplicationUser_;
using SnaelyFashion_Models;
using SnaelyFashion_Utility;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using SnaelyFashion_WebAPI.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;



namespace SnaelyFashion_AdminMVC.Controllers
{
    
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ApplicationDbContext _Context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public AccountController(IAuthService authService, ApplicationDbContext context, SignInManager<ApplicationUser>signInManager)
        {
            _authService = authService;
            _Context = context;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Login()
        {
            LoginRequestDTO obj = new();
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginRequestDTO obj)
        {
            var result = await _signInManager.PasswordSignInAsync(obj.Username, obj.Password, obj.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {


               
                return RedirectToAction("Index", "Home");
            }

        
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(obj);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            var obj = new RegisterationRequestDTO();
            return View(obj);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterationRequestDTO obj)
        {
            APIResponse result = await _authService.RegisterAsync<APIResponse>(obj);
            if (result != null && result.IsSuccess)
            {
                return RedirectToAction("Login");
            }
            return View();
        }


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
          await  _signInManager?.SignOutAsync();
           
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
