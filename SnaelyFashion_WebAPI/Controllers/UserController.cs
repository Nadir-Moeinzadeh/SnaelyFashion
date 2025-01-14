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
using SnaelyFashion_WebAPI.DataAccess.Repository.IRepository;
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
        private readonly IUnitOfWork _unitOfWork;
        public UserController(UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment,ApplicationDbContext dbContext, IUnitOfWork unitOfWork)
        {
            _context = dbContext;
            _response = new();
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("GetUserInfo")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetUserInfo()
        {
            try
            {
                //var user = await _userManager.GetUserAsync(User);


                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                var user = await _unitOfWork.ApplicationUser.GetAsync(x => x.Id == userId, includeProperties: "ProfilePicture");

                if (user == null) 
                {
                    return NotFound();
                }


                var ProfileInfo = new ProfileDTO()
                {
                    ID = userId,
                    FullName = $"{user.FirstName} {user.LastName}",
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    City = user.City,
                    PostalCode = user.PostalCode,
                    PhoneNumber = user.PhoneNumber,
                    State = user.State,
                    StreetAddress = user.StreetAddress,
                    ProfilePictureURL = user.ProfilePicture.ImageUrl,
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
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<APIResponse> EditUserInfo([FromBody]ProfileEditDTO editDTO) 
        {
            try
            {
                


                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                var user = await _unitOfWork.ApplicationUser.GetAsync(x => x.Id == userId, includeProperties: "ProfilePicture");



                var ProfileInfo = new ProfileDTO()
                {
                    ID = userId,
                    FullName = $"{user.FirstName} {user.LastName}",
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    City = user.City,
                    PostalCode = user.PostalCode,
                    PhoneNumber = user.PhoneNumber,
                    State = user.State,
                    StreetAddress = user.StreetAddress,
                    ProfilePictureURL = user.ProfilePicture.ImageUrl
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








                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {


                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string userPath = @"images\users\user-" + user.Id;
                    string finalPath = Path.Combine(wwwRootPath, userPath);

                    if (!Directory.Exists(finalPath))
                        Directory.CreateDirectory(finalPath);

                    using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    ProfilePicture profilepicture = new()
                    {
                        ImageUrl = @"\" + userPath + @"\" + fileName,
                        ApplicationUserId = user.Id,
                    };
                    _unitOfWork.ProfilePicture.Add(profilepicture);
                    _unitOfWork.Save();

                    if (user.ProfilePicture == null)
                        user.ProfilePicture = new ProfilePicture();

                    user.ProfilePicture=profilepicture;
                   



                    _unitOfWork.ApplicationUser.UpdateAsync(user);

                    _unitOfWork.Save();



                }


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



        [HttpPut("UploadProfilePicture")]
        public async Task<APIResponse> UploadProfilePicture( IFormFile? file)
        {
            try
            {



                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                var user = await _unitOfWork.ApplicationUser.GetAsync(x => x.Id == userId, includeProperties: "ProfilePicture");
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {


                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string userPath = @"images\users\user-" + user.Id;
                    string finalPath = Path.Combine(wwwRootPath, userPath);

                    if (!Directory.Exists(finalPath))
                        Directory.CreateDirectory(finalPath);

                    using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    ProfilePicture profilepicture = new()
                    {
                        ImageUrl = @"\" + userPath + @"\" + fileName,
                        ApplicationUserId = user.Id,
                    };
                    _unitOfWork.ProfilePicture.Add(profilepicture);
                    _unitOfWork.Save();

                    if (user.ProfilePicture == null)
                        user.ProfilePicture = new ProfilePicture();

                    user.ProfilePicture = profilepicture;




                    _unitOfWork.ApplicationUser.UpdateAsync(user);

                    _unitOfWork.Save();



                }


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


        [HttpDelete("DeleteProfilePicture")]
        public async Task<APIResponse> DeleteProfilePictureAsync()
        {

            try
            {

                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                var user = await _unitOfWork.ApplicationUser.GetAsync(x => x.Id == userId, includeProperties: "ProfilePicture");



                var imageToBeDeleted = user.ProfilePicture;
                if (imageToBeDeleted == null) 
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = System.Net.HttpStatusCode.NotFound;

                }

                if (imageToBeDeleted != null)
                {
                    if (!string.IsNullOrEmpty(imageToBeDeleted.ImageUrl))
                    {
                        var oldImagePath =
                                       Path.Combine(_webHostEnvironment.WebRootPath,
                                       imageToBeDeleted.ImageUrl.TrimStart('\\'));

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    _unitOfWork.ProfilePicture.Remove(imageToBeDeleted);
                    _unitOfWork.Save();
                    _response.IsSuccess = true;
                    _response.StatusCode = System.Net.HttpStatusCode.NoContent;

                }
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
