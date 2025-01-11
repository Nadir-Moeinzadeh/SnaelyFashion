using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SnaelyFashion_AdminMVC.Models;
using SnaelyFashion_Models;
using SnaelyFashion_Utility;
using SnaelyFashion_WebAPI.DataAccess.Data;
using SnaelyFashion_WebAPI.DataAccess.Repository.IRepository;
using System.Diagnostics;

namespace SnaelyFashion_AdminMVC.Controllers
{
    [Authorize(Roles = SD.Role_Admin)]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _Context;
        public HomeController(ApplicationDbContext dbContext, IWebHostEnvironment webHostEnvironment, IUnitOfWork unitOfWork)
        {
           _Context = dbContext;
            _webHostEnvironment = webHostEnvironment;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<BlogPost> productList = _unitOfWork.BlogPost.GetAll(includeProperties: "blogPostImages").OrderBy(x=>x.CreatedDate).Reverse();
            return View(productList);
        }

        public async Task<IActionResult> Create() 
        {
            var blogpostVM = new BlogPostVM();
            return View(blogpostVM);
        }

        [HttpPost]
        public async Task<IActionResult> Create(BlogPostVM blogpostVM, List<IFormFile>? files)
        {


            blogpostVM.BlogPost.CreatedDate= DateTime.Now;
            _unitOfWork.BlogPost.Add(blogpostVM.BlogPost);
            _unitOfWork.Save();

            string wwwRootPath = _webHostEnvironment.WebRootPath;
            if (files != null)
            {

                foreach (IFormFile file in files)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string BlogpostPath = @"images\BlogPosts\blogpost-" + blogpostVM.BlogPost.Id;
                    string finalPath = Path.Combine(wwwRootPath, BlogpostPath);

                    if (!Directory.Exists(finalPath))
                        Directory.CreateDirectory(finalPath);

                    using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    BlogPostImage blogpostImage = new()
                    {
                        ImageUrl = @"\" + BlogpostPath + @"\" + fileName,
                        BlogPostId = blogpostVM.BlogPost.Id,
                    };
                    _unitOfWork.BlogPostImage.Add(blogpostImage);
                    _unitOfWork.Save();

                    if (blogpostVM.BlogPost.blogPostImages == null)
                        blogpostVM.BlogPost.blogPostImages = new List<BlogPostImage>();

                    blogpostVM.BlogPost.blogPostImages.Add(blogpostImage);
                    _unitOfWork.Save();

                }

                 _unitOfWork.BlogPost.Update(blogpostVM.BlogPost);

                _unitOfWork.Save();



            }


            TempData["success"] = "Blogpost Posted successfully";
            return RedirectToAction("Index");

        }









        public async Task<IActionResult> Upsert(int? id)
        {


            var postblogToUpdate = _unitOfWork.BlogPost.Get(u => u.Id == id, includeProperties: "blogPostImages");
          
            var postblogImages = _Context.BlogPostImages.Where(u => u.BlogPostId == id).ToList();

            var entity = _Context.BlogPosts.Where(u => u.Id == id).FirstOrDefault();

            var date = entity.CreatedDate;

            postblogToUpdate.blogPostImages = postblogImages;
            postblogToUpdate.CreatedDate = date;


            BlogPostVM blogpostVM = new()
            {
               
            };

          

            //update
            blogpostVM.BlogPost = postblogToUpdate;
           
            return View(blogpostVM);


        }
        [HttpPost]
        public async Task<IActionResult> Upsert(BlogPostVM blogpostVM, List<IFormFile> files)
        {
           
            
          
            
          
            var blogpostToEdit = blogpostVM.BlogPost;
            var date = blogpostToEdit.CreatedDate;
           


            _unitOfWork.BlogPost.Update(blogpostToEdit);


            _unitOfWork.Save();


            string wwwRootPath = _webHostEnvironment.WebRootPath;
            if (files != null)
            {

                foreach (IFormFile file in files)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string blogpostPath = @"images\BlogPosts\blogpost-" + blogpostVM.BlogPost.Id;
                    string finalPath = Path.Combine(wwwRootPath, blogpostPath);

                    if (!Directory.Exists(finalPath))
                        Directory.CreateDirectory(finalPath);

                    using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    BlogPostImage blogpostImage = new()
                    {
                        ImageUrl = @"\" + blogpostPath + @"\" + fileName,
                        BlogPostId = blogpostVM.BlogPost.Id,
                    };
                    _unitOfWork.BlogPostImage.Add(blogpostImage);
                    _unitOfWork.Save();
                    if (blogpostVM.BlogPost.blogPostImages == null)
                        blogpostVM.BlogPost.blogPostImages = new List<BlogPostImage>();

                    blogpostVM.BlogPost.blogPostImages.Add(blogpostImage);

                }
                blogpostToEdit.CreatedDate = date;
                _unitOfWork.BlogPost.Update(blogpostToEdit);
                _unitOfWork.Save();




            }


            TempData["success"] = "Blogpost updated successfully";
            return RedirectToAction("Index");

        }




        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            BlogPost? blogpostFromDb = await _unitOfWork.BlogPost.GetAsync(u => u.Id == id);

            if (blogpostFromDb == null)
            {
                return NotFound();
            }
            return View(blogpostFromDb);
        }




        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeletePOST(int? id)
        {
            BlogPost? obj = await _unitOfWork.BlogPost.GetAsync(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            var blogpostToBeDeleted = _unitOfWork.BlogPost.Get(u => u.Id == id);
            if (blogpostToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            string blogpostPath = @"images\BlogPosts\blogpost-" + id;
            string finalPath = Path.Combine(_webHostEnvironment.WebRootPath, blogpostPath);

            if (Directory.Exists(finalPath))
            {
                string[] filePaths = Directory.GetFiles(finalPath);
                foreach (string filePath in filePaths)
                {
                    System.IO.File.Delete(filePath);
                }

                Directory.Delete(finalPath);
            }



            _unitOfWork.BlogPost.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Blogpost deleted successfully!";
            return RedirectToAction("Index");
        }




        public IActionResult DeleteImage(int imageId)
        {
            var imageToBeDeleted = _unitOfWork.BlogPostImage.Get(u => u.Id == imageId);
            int blogpostId = imageToBeDeleted.BlogPostId;
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

                _unitOfWork.BlogPostImage.Remove(imageToBeDeleted);
                _unitOfWork.Save();

                TempData["success"] = "Deleted successfully";
            }

            return RedirectToAction(nameof(Upsert), new { id = blogpostId });
        }

    }
}
