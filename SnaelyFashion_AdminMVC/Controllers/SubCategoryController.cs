using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SnaelyFashion_AdminMVC.Models;
using SnaelyFashion_Models;
using SnaelyFashion_Utility;
using SnaelyFashion_WebAPI.DataAccess.Repository.IRepository;

namespace SnaelyFashion_AdminMVC.Controllers
{
    [Authorize(Roles = SD.Role_Admin)]
    public class SubCategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public SubCategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<IActionResult> Index()
        {
            List<SubCategory> objSubCategoryList = await _unitOfWork.SubCategory.GetAllAsync(includeProperties:"Category");
            return View(objSubCategoryList);

        }

        public IActionResult Create()
        {
            SubCategoryVM subCategoryVM = new()
            {
                CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                SubCategory = new SubCategory()
            };


            return View(subCategoryVM);
        }
        [HttpPost]
        public async Task<IActionResult> Create(SubCategoryVM obj)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.SubCategory.CreateAsync(obj.SubCategory);

                _unitOfWork.Save();
                TempData["success"] = "SubCategory created successfully";
                return RedirectToAction("Index");

            }
            else
            {
                obj.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(obj);
            }




          

        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            SubCategory? SubcategoryFromDb = await _unitOfWork.SubCategory.GetAsync(u => u.Id == id,includeProperties:"Category");
            SubCategoryVM subCategoryVM = new()
            {
                CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
              SubCategory  = new SubCategory()
            };

            if (id == null || id == 0)
            {
                //create
                return View(subCategoryVM);
            }
            else
            {
                //update
                subCategoryVM.SubCategory = _unitOfWork.SubCategory.Get(u => u.Id == id);
                return View(subCategoryVM);
            }

        }
        [HttpPost]
        public async Task<IActionResult> Edit(SubCategoryVM obj)
        {
            if (ModelState.IsValid)
            {
                if (obj.SubCategory.Id == 0)
                {
                    _unitOfWork.SubCategory.Add(obj.SubCategory);
                }
                else
                {
                    await _unitOfWork.SubCategory.UpdateAsync(obj.SubCategory);
                }

                _unitOfWork.Save();
                return RedirectToAction("Index");

            }
            else
            {
                obj.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(obj);
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            SubCategory? categoryFromDb = await _unitOfWork.SubCategory.GetAsync(u => u.Id == id);

            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeletePOST(int? id)
        {
            SubCategory? obj = await _unitOfWork.SubCategory.GetAsync(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _unitOfWork.SubCategory.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "SubCategory deleted successfully!";
            return RedirectToAction("Index");
        }
    }
}
