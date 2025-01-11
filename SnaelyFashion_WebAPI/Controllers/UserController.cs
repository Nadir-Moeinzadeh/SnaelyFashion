using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SnaelyFashion_Models;
using SnaelyFashion_Models.DTO.ApplicationUser_;
using SnaelyFashion_Models.DTO.Profile_;
using SnaelyFashion_WebAPI.DataAccess.Data;
using System.Security.Claims;

namespace SnaelyFashion_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class UserController : ControllerBase
    {
        protected APIResponse _response;
        private readonly UserManager<ApplicationUser> _userManager;
        private ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public UserController(UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment,ApplicationDbContext dbContext)
        {
            _context = dbContext;
            _response = new();
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("GetUserInfo")]
        
        public async Task<ActionResult<APIResponse>> GetUserInfo()
        {
            try
            {
                //var user = await _userManager.GetUserAsync(User);


                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                var user = await _context.ApplicationUsers.Where(x => x.Id == userId).FirstOrDefaultAsync();

                if (user == null) 
                {
                    return NotFound();
                }


                var ProfileInfo = new ProfileDTO()
                {
                   ID = userId,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    City = user.City,
                    PostalCode = user.PostalCode,
                    PhoneNumber = user.PhoneNumber,
                    State = user.State,
                    StreetAddress = user.StreetAddress,
                    ProfilePictureURL = user.ProfilePictureURL
                };

                _response.IsSuccess = true;
                _response.Result = ProfileInfo;
                _response.StatusCode = System.Net.HttpStatusCode.OK;

                return Ok(_response);
            }
            catch (Exception ex) 
            {
                _response.IsSuccess = false;
                _response.ErrorMessages
                     = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpPut("EditUserInfo")]
        public async Task<APIResponse> EditUserInfo([FromBody]ProfileEditDTO editDTO) 
        {
            try
            {
                


                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                var user = await _context.ApplicationUsers.Where(x => x.Id == userId).FirstOrDefaultAsync();



                var ProfileInfo = new ProfileDTO()
                {
                    ID = userId,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    City = user.City,
                    PostalCode = user.PostalCode,
                    PhoneNumber = user.PhoneNumber,
                    State = user.State,
                    StreetAddress = user.StreetAddress,
                    ProfilePictureURL = user.ProfilePictureURL
                };

                if (editDTO.FirstName != ProfileInfo.FirstName) { ProfileInfo.FirstName = editDTO.FirstName; }
                if (editDTO.LastName != ProfileInfo.LastName) { ProfileInfo.LastName = editDTO.LastName; }
                if (editDTO.City != ProfileInfo.City) { ProfileInfo.City = editDTO.City; }
                if (editDTO.PostalCode != ProfileInfo.PostalCode) { ProfileInfo.PostalCode = editDTO.PostalCode; }
                if (editDTO.PhoneNumber != ProfileInfo.PhoneNumber) { ProfileInfo.PhoneNumber = editDTO.PhoneNumber; }
                if (editDTO.State != ProfileInfo.State) { ProfileInfo.State = editDTO.State; }
                if (editDTO.StreetAddress != ProfileInfo.StreetAddress) { ProfileInfo.StreetAddress = editDTO.StreetAddress; }

                user.FirstName = ProfileInfo.FirstName;
                user.LastName = ProfileInfo.LastName;
                user.City = ProfileInfo.City;
                user.PostalCode = ProfileInfo.PostalCode;
                user.PhoneNumber = ProfileInfo.PhoneNumber;
                user.State = ProfileInfo.State;
                user.StreetAddress = ProfileInfo.StreetAddress;

                 _context.ApplicationUsers.Update(user);
                _context.SaveChanges();




                _response.IsSuccess = true;
                _response.Result = editDTO;
                _response.StatusCode = System.Net.HttpStatusCode.OK;

                return _response;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages
                     = new List<string>() { ex.ToString() };
            }
            return _response;

        }

    }
}
