using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SnaelyFashion_AdminMVC.Models;
using SnaelyFashion_Models;
using SnaelyFashion_Utility;
using SnaelyFashion_WebAPI.DataAccess.Data;
using SnaelyFashion_WebAPI.DataAccess.Repository.IRepository;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace SnaelyFashion_AdminMVC.Controllers
{
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
       
        private readonly ApplicationDbContext _context;
        public UserController(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork, RoleManager<IdentityRole> roleManager,  ApplicationDbContext context)
        {
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
            _userManager = userManager;
           

        }
        public IActionResult Index()
        {
            return View();
        }







        public async Task<IActionResult> RoleManagment(string userId)
        {

            RoleManagmentVM RoleVM = new RoleManagmentVM()
            {
                ApplicationUser =await _unitOfWork.ApplicationUser.GetAsync(u => u.Id == userId),
                RoleList = _roleManager.Roles.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Name
                }),
              
            };

            RoleVM.ApplicationUser.Role = _userManager.GetRolesAsync(await _unitOfWork.ApplicationUser.GetAsync(u => u.Id == userId))
                    .GetAwaiter().GetResult().FirstOrDefault();
            return View(RoleVM);
        }

        [HttpPost]
        public async Task<IActionResult> RoleManagment(RoleManagmentVM roleManagmentVM)
        {

            string oldRole = _userManager.GetRolesAsync(await _unitOfWork.ApplicationUser.GetAsync(u => u.Id == roleManagmentVM.ApplicationUser.Id))
                    .GetAwaiter().GetResult().FirstOrDefault();

            ApplicationUser applicationUser =await _unitOfWork.ApplicationUser.GetAsync(u => u.Id == roleManagmentVM.ApplicationUser.Id);


            if (!(roleManagmentVM.ApplicationUser.Role == oldRole))
            {

               
              await  _unitOfWork.ApplicationUser.UpdateAsync(applicationUser);
                _unitOfWork.Save();

                _userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(applicationUser, roleManagmentVM.ApplicationUser.Role).GetAwaiter().GetResult();

            }
         

            return RedirectToAction("Index");
        }


        #region API CALLS

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            List<ApplicationUser> objUserList =await _unitOfWork.ApplicationUser.GetAllAsync();

            foreach (var user in objUserList)
            {

                user.Role = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();

             
            }

            return Json(new { data = objUserList });
        }


        [HttpPost]
        public async Task<IActionResult> LockUnlock([FromBody] string id)
        {

            var objFromDb =await _unitOfWork.ApplicationUser.GetAsync(u => u.Id == id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while Locking/Unlocking" });
            }

            if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
            {
                //user is currently locked and we need to unlock them
                objFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
            }
           await _unitOfWork.ApplicationUser.UpdateAsync(objFromDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Operation Successful" });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] string id)
        {

            var objFromDb = await _unitOfWork.ApplicationUser.GetAsync(u => u.Id == id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while Locking/Unlocking" });
            }

            await _userManager.DeleteAsync(objFromDb);
            await _unitOfWork.ApplicationUser.DeleteUser(objFromDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Operation Successful" });
        }


        #endregion
    }
}
