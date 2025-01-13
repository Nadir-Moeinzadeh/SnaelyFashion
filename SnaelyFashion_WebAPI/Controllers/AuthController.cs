using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SnaelyFashion_Models;

using SnaelyFashion_Models.DTO.ApplicationUser_;
using SnaelyFashion_Utility;
using SnaelyFashion_WebAPI.DataAccess.Data;
using SnaelyFashion_WebAPI.DataAccess.Repository.IRepository;
using System.Net;

namespace SnaelyFashion_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        protected APIResponse _response;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private ApplicationDbContext _context;
        public AuthController(IUserRepository userRepo, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser>userManager,RoleManager<IdentityRole>roleManager,ApplicationDbContext dbContext)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _userRepo = userRepo;
            _roleManager = roleManager;
            _response = new();
            _context = dbContext;
        }



        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        {
            var loginResponse = await _userRepo.Login(model);

            var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, lockoutOnFailure: false);
            var _user = await _userManager.FindByNameAsync(model.Username);
            if (result.Succeeded) 
            {
                var user = loginResponse.User;
                var role = user.Roles.FirstOrDefault();
                loginResponse.role = role;
                _signInManager.SignInAsync(_user, false, "Bearer");
            }

            if (loginResponse.User == null || string.IsNullOrEmpty(loginResponse.Token))
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Username or password is incorrect");
                return BadRequest(_response);
            }
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Result = loginResponse;
            return Ok(_response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterationRequestDTO model)
        {
            bool ifUserNameUnique = _userRepo.IsUniqueUser(model.UserName);
            if (!ifUserNameUnique)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Username already exists");
                return BadRequest(_response);
            }

            var user = await _userRepo.Register(model);
            if (user == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Error while registering");
                return BadRequest(_response);
            }
            if (user != null)
            {

                var CreatedUser = _context.ApplicationUsers.FirstOrDefault(x => x.Id == user.ID);
                await _userManager.AddToRoleAsync(CreatedUser, SD.Role_Customer);
            }
            
            
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            return Ok(_response);
        }

       
        [HttpPost("LogOut")]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }
    }
}
