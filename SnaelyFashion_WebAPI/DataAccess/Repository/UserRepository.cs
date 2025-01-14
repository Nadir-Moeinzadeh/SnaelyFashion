using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SnaelyFashion_Models.DTO.ApplicationUser_;
using SnaelyFashion_Models;
using SnaelyFashion_Utility;
using SnaelyFashion_WebAPI.DataAccess.Data;
using SnaelyFashion_WebAPI.DataAccess.Repository.IRepository;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;


namespace SnaelyFashion_WebAPI.DataAccess.Repository
{
    public class UserRepository : IUserRepository
    {


        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private string _secretKey;
        private readonly IMapper _mapper;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public UserRepository(ApplicationDbContext db, IConfiguration configuration,
            UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _signInManager = signInManager;
            _userManager = userManager;
            _secretKey = configuration.GetValue<string>("ApiSettings:Secret");
            _roleManager = roleManager;
        }

        public bool IsUniqueUser(string username)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(x => x.UserName == username);
            if (user == null)
            {
                return true;
            }
            return false;
        }

        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            var user =await _db.ApplicationUsers
                .FirstOrDefaultAsync(u => u.UserName.ToLower() == loginRequestDTO.Username.ToLower());

            

            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);

          



            if (user == null || isValid == false)
            {
                return new LoginResponseDTO()
                {
                    Token = "",
                    User = null
                };
            }
           
            //if user was found it generates JWT Token
          
            var roles = await _userManager.GetRolesAsync(user);
            var roleslist = roles.ToList();
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secretKey);
            var _role = roleslist.FirstOrDefault();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {   new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName.ToString()),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault())
                }),
                Expires = DateTime.UtcNow.AddMinutes(100),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            LoginResponseDTO loginResponseDTO = new LoginResponseDTO()
            {
                Token = tokenHandler.WriteToken(token),
                User = new UserDTO() { ID=user.Id,UserName=user.UserName,FirstName=user.FirstName,LastName=user.LastName,Roles=roleslist},
                role=_role.ToString()
            };
           
            return loginResponseDTO;
        }

        public async Task<UserDTO> Register(RegisterationRequestDTO registerationRequestDTO)
        {
            ApplicationUser user = new()
            {
                UserName = registerationRequestDTO.UserName,
                Email = registerationRequestDTO.UserName,
                FirstName = registerationRequestDTO.FirstName,
                LastName = registerationRequestDTO.LastName,
           
            };

            try
            {
               
                    var result = await _userManager.CreateAsync(user, registerationRequestDTO.Password);

                    if (result.Succeeded)
                    {
                        if (!_roleManager.RoleExistsAsync("Admin").GetAwaiter().GetResult())
                        {
                            _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();
                            _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();
                            _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();

                        }
                        await _userManager.AddToRoleAsync(user, SD.Role_Customer);
                        var userToReturn = _db.ApplicationUsers
                            .FirstOrDefault(u => u.UserName == registerationRequestDTO.UserName);
                    var userDTO = new UserDTO() { ID = userToReturn.Id, UserName = userToReturn.UserName, FirstName = userToReturn.FirstName, LastName = userToReturn.LastName };
                        return userDTO;

                    }


              
            }
            catch (Exception e)
            {

            }

            return new UserDTO();
        }


        public async Task UpdateAsync(ApplicationUser applicationUser)
        {
            _db.ApplicationUsers.Update(applicationUser);
            await _db.SaveChangesAsync();
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }

        public async Task SignOut()
        {
            await _signInManager.SignOutAsync();
            
        }
    }
}
