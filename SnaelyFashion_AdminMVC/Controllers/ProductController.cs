using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp;
using SnaelyFashion_AdminMVC.Models;
using SnaelyFashion_Models;
using SnaelyFashion_Utility;
using SnaelyFashion_WebAPI.DataAccess.Data;
using SnaelyFashion_WebAPI.DataAccess.Repository.IRepository;

namespace SnaelyFashion_AdminMVC.Controllers
{
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _Context;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment, ApplicationDbContext dbContext)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            _Context = dbContext;
        }
        public async Task<IActionResult> Index()
        {
            List<Product> objProductList =await _unitOfWork.Product.GetAllAsync(includeProperties: "Category,SubCategory");
            return View(objProductList);
        }


        public IActionResult Create()
        {
           

            var _SizesCheckBoxes = new List<CheckboxOption>();
           
            var _ShoesSizesCheckBoxes = new List<CheckboxOption>();
            var _ColorsCheckBoxes = new List<CheckboxOption>();


            foreach (var size in SD.Sizes)
            {
                var _SizeCheckbox = new CheckboxOption 
                { 
                    IsChecked = false,
                    Value = size,
                    Description=size
                
                };
                _SizesCheckBoxes.Add(_SizeCheckbox);
            }

            foreach (var shoesize in SD.ShoeSizes)
            {
                var _ShoeSizeCheckbox = new CheckboxOption
                {
                    IsChecked = false,
                    Value = shoesize,
                    Description = shoesize

                };
                _ShoesSizesCheckBoxes.Add(_ShoeSizeCheckbox);
            }
          

            foreach (var color in SD.Colors)
            {
                var _ColorCheckbox = new CheckboxOption
                {
                    IsChecked = false,
                    Value = color,
                    Description = color

                };
                _ColorsCheckBoxes.Add(_ColorCheckbox);
            }






            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                SubCategoryList = _unitOfWork.SubCategory.GetAll().Select(u => new SelectListItem
                {
                    Text = u.SubCategoryName,
                    Value = u.Id.ToString()
                }),
                Gender = SD.Genders.Select(u => new SelectListItem
                {
                    Text = u,
                    Value = u

                }),

                Product = new Product(),
                SizeCheckBoxes=_SizesCheckBoxes,
                ShoeSizeCheckBoxes=_ShoesSizesCheckBoxes,
               
                ColorsCheckBoxes=_ColorsCheckBoxes,
                SelectedColors=new List<string>(),
                SelectedSizes=new List<string>()

               
               
            };
            
                //create
                return View(productVM);
         
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductVM productVM, List<IFormFile>? files)
        {
            
               var SelectedColorsList = productVM.SelectedColors;
                var SelectedSizesList = productVM.SelectedSizes;
               
                var ColorsList = new List<ProductColor>();
                var SizesList = new List<ProductSize>();

                _unitOfWork.Product.Add(productVM.Product);
                _unitOfWork.Save();

              
              var CreatedProduct =  _unitOfWork.Product.Get(u => u.Title == productVM.Product.Title);


                foreach(var color in SelectedColorsList) 
                {
                    var ProductColor = new ProductColor()
                    {
                        ProductId = productVM.Product.Id,
                        Color = color
                    
                    };

                    ColorsList.Add(ProductColor);
                }
                foreach (var Size in SelectedSizesList)
                {
                    var ProductSize = new ProductSize()
                    {
                        ProductId = productVM.Product.Id,
                        Size = Size

                    };

                    SizesList.Add(ProductSize);
                }

                _unitOfWork.ProductColor.AddRange(ColorsList);
                _unitOfWork.Save();
                _unitOfWork.ProductSize.AddRange(SizesList);
                _unitOfWork.Save();







            string wwwRootPath = SD.Defaultwwwroot;
            if (files != null)
                {

                    foreach (IFormFile file in files)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string productPath = @"images\products\product-" + productVM.Product.Id;
                        string finalPath = Path.Combine(wwwRootPath, productPath);

                        if (!Directory.Exists(finalPath))
                            Directory.CreateDirectory(finalPath);

                        using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        ProductImage productImage = new()
                        {
                            ImageUrl = @"\" + productPath + @"\" + fileName,
                            ProductId = productVM.Product.Id,
                        };
                        _unitOfWork.ProductImage.Add(productImage);
                        _unitOfWork.Save();

                        if (productVM.Product.ProductImages == null)
                            productVM.Product.ProductImages = new List<ProductImage>();

                        productVM.Product.ProductImages.Add(productImage);
                        _unitOfWork.Save();

                    }

                    await _unitOfWork.Product.UpdateAsync(productVM.Product);

                    _unitOfWork.Save();



                }


                TempData["success"] = "Product created successfully";
                return RedirectToAction("Index");
           
        }






        public IActionResult Upsert(int? id)
        {


            var ProductToUpdate = _unitOfWork.Product.Get(u=>u.Id == id,includeProperties: "Sizes,Colors,ProductImages");
            var ExistingColors = ProductToUpdate.Colors.Select(x=>x.Color).ToList();
            var ExistingSizes = ProductToUpdate.Sizes.Select(x => x.Size).ToList();
            var ProductImages = _Context.ProductImages.Where(u=>u.ProductId == id).ToList();    

            ProductToUpdate.ProductImages = ProductImages;


            var _SizesCheckBoxes = new List<CheckboxOption>();
          
            var _ShoesSizesCheckBoxes = new List<CheckboxOption>();
            var _ColorsCheckBoxes = new List<CheckboxOption>();


            foreach (var size in SD.Sizes)
            {
                if (!ExistingSizes.Contains(size)) 
                {
                      var _SizeCheckbox = new CheckboxOption
                      {
                    
                        IsChecked = false,
                        Value = size,
                        Description = size

                      };

                     _SizesCheckBoxes.Add(_SizeCheckbox);
                }

                if (ExistingSizes.Contains(size))
                {

                    var _SizeCheckbox = new CheckboxOption
                    {

                        IsChecked = true,
                        Value = size,
                        Description = size

                    };

                    _SizesCheckBoxes.Add(_SizeCheckbox);
                }
            }

            foreach (var shoesize in SD.ShoeSizes)
            {
                if (!ExistingSizes.Contains(shoesize)) 
                { 


                     var _ShoeSizeCheckbox = new CheckboxOption
                     {
                      IsChecked = false,
                      Value = shoesize,
                      Description = shoesize

                     };
                 _ShoesSizesCheckBoxes.Add(_ShoeSizeCheckbox);
                }
                if (ExistingSizes.Contains(shoesize))
                {


                    var _ShoeSizeCheckbox = new CheckboxOption
                    {
                        IsChecked = true,
                        Value = shoesize,
                        Description = shoesize

                    };
                    _ShoesSizesCheckBoxes.Add(_ShoeSizeCheckbox);
                }
            }
        

            foreach (var color in SD.Colors)
            {
                if(!ExistingColors.Contains(color))
                {
                    var _ColorCheckbox = new CheckboxOption
                    {
                        IsChecked = false,
                        Value = color,
                        Description = color

                    };
                    _ColorsCheckBoxes.Add(_ColorCheckbox);
                }

                if (ExistingColors.Contains(color))
                {
                    var _ColorCheckbox = new CheckboxOption
                    {
                        IsChecked = true,
                        Value = color,
                        Description = color

                    };
                    _ColorsCheckBoxes.Add(_ColorCheckbox);
                }
            }

            








            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                SubCategoryList = _unitOfWork.SubCategory.GetAll().Select(u => new SelectListItem
                {
                    Text = u.SubCategoryName,
                    Value = u.Id.ToString()
                }),
                Gender = SD.Genders.Select(u => new SelectListItem
                {
                    Text = u,
                    Value=u
                    
                }),
                Product = ProductToUpdate,
                SizeCheckBoxes = _SizesCheckBoxes,
                ShoeSizeCheckBoxes = _ShoesSizesCheckBoxes,

                ColorsCheckBoxes = _ColorsCheckBoxes,
                SelectedColors = new List<string>(),
                SelectedSizes = new List<string>()
            };
          
                //update
                productVM.Product = _unitOfWork.Product.Get(u => u.Id == id, includeProperties: "Sizes,Colors,ProductImages");
                return View(productVM);
           

        }
        [HttpPost]
        public async Task<IActionResult> Upsert(ProductVM productVM, List<IFormFile> files)
        {
            var SelectedColorsList = productVM.SelectedColors;
            var SelectedSizesList = productVM.SelectedSizes;
            var productImages = productVM.Product.ProductImages;
            var ColorsList = new List<ProductColor>();
            var SizesList = new List<ProductSize>();

            //var ProductToEdit = _unitOfWork.Product.Get(u => u.Id == productVM.Product.Id,includeProperties: "Sizes,Colors");

            var ProductToEdit = productVM.Product;


            var ExistingColors = await _unitOfWork.ProductColor.GetAllAsync(u=>u.ProductId==productVM.Product.Id);
            var ExistingSizes = await _unitOfWork.ProductSize.GetAllAsync(u => u.ProductId == productVM.Product.Id);
            if (ExistingColors != null)
            {
                _unitOfWork.ProductColor.RemoveRange(ExistingColors);
                _unitOfWork.Save();
            }
            if (ExistingSizes != null)
            {
                _unitOfWork.ProductSize.RemoveRange(ExistingSizes);

                _unitOfWork.Save();
            }

            foreach (var color in SelectedColorsList)
            {
                var ProductColor = new ProductColor()
                {
                    ProductId = productVM.Product.Id,
                    Color = color

                };

                ColorsList.Add(ProductColor);
            }
            foreach (var Size in SelectedSizesList)
            {
                var ProductSize = new ProductSize()
                {
                    ProductId = productVM.Product.Id,
                    Size = Size

                };

                SizesList.Add(ProductSize);
            }

            _unitOfWork.ProductColor.AddRange(ColorsList);
           
            _unitOfWork.ProductSize.AddRange(SizesList);
            _unitOfWork.Save();

            var NewCreatedColors = _Context.ProductColors.Where(c => c.ProductId==ProductToEdit.Id).ToList();
            var NewCreatedSizess = _Context.ProductSizes.Where(c => c.ProductId == ProductToEdit.Id).ToList();

            ProductToEdit.Colors = NewCreatedColors;
            ProductToEdit.Sizes = NewCreatedSizess;



            _unitOfWork.Product.Update(ProductToEdit);
              

                _unitOfWork.Save();


            string wwwRootPath = SD.Defaultwwwroot;
            if (files != null)
                {

                    foreach (IFormFile file in files)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string productPath = @"images\products\product-" + productVM.Product.Id;
                        string finalPath = Path.Combine(wwwRootPath, productPath);

                        if (!Directory.Exists(finalPath))
                            Directory.CreateDirectory(finalPath);

                        using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        ProductImage productImage = new()
                        {
                            ImageUrl = @"\" + productPath + @"\" + fileName,
                            ProductId = productVM.Product.Id,
                        };
                         _unitOfWork.ProductImage.Add(productImage);
                        _unitOfWork.Save();
                        if (productVM.Product.ProductImages == null)
                            productVM.Product.ProductImages = new List<ProductImage>();

                        productVM.Product.ProductImages.Add(productImage);

                    }

                    _unitOfWork.Product.Update(ProductToEdit);
                    _unitOfWork.Save();




                }


                TempData["success"] = "Product updated successfully";
                return RedirectToAction("Index");
           
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product? categoryFromDb = await _unitOfWork.Product.GetAsync(u => u.Id == id);

            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeletePOST(int? id)
        {
            Product? obj = await _unitOfWork.Product.GetAsync(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            var productToBeDeleted = _unitOfWork.Product.Get(u => u.Id == id);
            if (productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            string wwwRootPath = SD.Defaultwwwroot;
            string productPath = @"images\products\product-" + id;
            string finalPath = Path.Combine(wwwRootPath, productPath);

            if (Directory.Exists(finalPath))
            {
                string[] filePaths = Directory.GetFiles(finalPath);
                foreach (string filePath in filePaths)
                {
                    System.IO.File.Delete(filePath);
                }

                Directory.Delete(finalPath);
            }



            _unitOfWork.Product.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "SubCategory deleted successfully!";
            return RedirectToAction("Index");
        }


        public IActionResult DeleteImage(int imageId)
        {
            var imageToBeDeleted = _unitOfWork.ProductImage.Get(u => u.Id == imageId);
            int productId = imageToBeDeleted.ProductId;
            string wwwRootPath = SD.Defaultwwwroot;
            if (imageToBeDeleted != null)
            {
                if (!string.IsNullOrEmpty(imageToBeDeleted.ImageUrl))
                {
                    var oldImagePath =
                                   Path.Combine(wwwRootPath,
                                   imageToBeDeleted.ImageUrl.TrimStart('\\'));

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                _unitOfWork.ProductImage.Remove(imageToBeDeleted);
                _unitOfWork.Save();

                TempData["success"] = "Deleted successfully";
            }

            return RedirectToAction(nameof(Upsert), new { id = productId });
        }





    }




}

