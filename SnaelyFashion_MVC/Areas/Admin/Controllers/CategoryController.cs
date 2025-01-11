using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SnaelyFashion_Models;
using SnaelyFashion_Utility;
using SnaelyFashion_WebAPI.DataAccess.Repository.IRepository;

namespace SnaelyFashion_MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index()
        {
            List<Category> objCategoryList = await _unitOfWork.Category.GetAllAsync();
            return View(objCategoryList);
        }
    }
}
