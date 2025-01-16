using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;
using SnaelyFashion_Models;
using SnaelyFashion_Models.DTO.Blogposts_;
using SnaelyFashion_Models.DTO.Product_;
using SnaelyFashion_Models.DTO.Review_;
using SnaelyFashion_Utility;
using SnaelyFashion_WebAPI.DataAccess.Data;
using SnaelyFashion_WebAPI.DataAccess.Repository.IRepository;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;

namespace SnaelyFashion_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        protected APIResponse _response;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _Context;
        public HomeController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment, ApplicationDbContext dbContext,IMapper mapper)
        {
            _Context = dbContext;
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            _response = new APIResponse();
            _mapper = mapper;
        }


        [HttpGet("Get3ofBlogposts")]
        [ResponseCache(CacheProfileName = "Default30")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> Get3ofBlogposts() 
        {
            try 
            {
                IEnumerable<BlogPost> blogpostList = _unitOfWork.BlogPost.GetAll(includeProperties: "blogPostImages").OrderBy(x => x.CreatedDate).Reverse();
                var leatest3blogs =blogpostList.Take(3).ToList();

                var blogpostDTOlist = new List<GetAllBlogPostsDTO>();

                foreach (var blogpost in leatest3blogs)
                {
                    var _ID = blogpost.Id;
                    var _title = blogpost.Title;
                    var _description = blogpost.Description;
                    var _blogpostimage = await _unitOfWork.BlogPostImage.GetAsync(u => u.BlogPostId == _ID);
                    var _blogpostimageUrl = _blogpostimage.ImageUrl;



                    var blogpostDTO = new GetAllBlogPostsDTO 
                    {
                        Id = _ID,
                        Title = _title,
                        Description = _description,
                        BlogpostImageUrl = SD.Defaultwwwroot+_blogpostimageUrl,
                        CreatedDate = blogpost.CreatedDate

                    };

                    blogpostDTOlist.Add(blogpostDTO);

                }


                _response.Result = blogpostDTOlist;

                _response.StatusCode = HttpStatusCode.OK;
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










        [HttpGet("GetAllProducts")]
        [ResponseCache(CacheProfileName = "Default30")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetAllProducts()
        {
            try
            {
                var products = await _unitOfWork.Product.GetAllAsync();

                var productlist = new List<AllProductsDTO>();

                foreach (var product in products) 
                {
                    




                    var _ID = product.Id;
                    var _title=product.Title;
                    var _description=product.Description;
                    var _price=product.Price;
                    var _categoryID = product.CategoryId;
                    var _subcategoryID = product.SubCategoryId;
                    var _gender = product.Gender;   
                    var _category = await _unitOfWork.Category.GetAsync(u=>u.Id==_categoryID);
                    var _subcategory = await _unitOfWork.SubCategory.GetAsync(u=>u.Id == _subcategoryID);
                    var _productimage = await _unitOfWork.ProductImage.GetAsync(u=>u.ProductId==_ID);
                    var _productimageUrl = SD.Defaultwwwroot + _productimage.ImageUrl;
                    var _categoryname = _category.Name;
                    var _subcategoryname = _subcategory.SubCategoryName;
                   




                    var productDTO = new AllProductsDTO
                    {
                        Id = _ID,
                        Title = _title,
                        CategoryId = _categoryID,
                        SubCategoryId = _subcategoryID,
                        Category=_categoryname,
                        SubCategory=_subcategoryname,
                        Description = _description,
                        Gender = _gender,
                        Price = _price,
                        ProductImageUrl = _productimageUrl,
                    
                    };
                    productlist.Add(productDTO);

                
                }

               





                _response.Result= productlist;
               
                _response.StatusCode = HttpStatusCode.OK;
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




        [HttpGet("GetAllMaleProducts")]
        [ResponseCache(CacheProfileName = "Default30")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetAllMaleProducts()
        {
            try
            {
                var products = await _unitOfWork.Product.GetAllAsync();

                var productlist = new List<AllProductsDTO>();

                foreach (var product in products)
                {





                    var _ID = product.Id;
                    var _title = product.Title;
                    var _description = product.Description;
                    var _price = product.Price;
                    var _categoryID = product.CategoryId;
                    var _subcategoryID = product.SubCategoryId;
                    var _gender = product.Gender;
                    var _category = await _unitOfWork.Category.GetAsync(u => u.Id == _categoryID);
                    var _subcategory = await _unitOfWork.SubCategory.GetAsync(u => u.Id == _subcategoryID);
                    var _productimage = await _unitOfWork.ProductImage.GetAsync(u => u.ProductId == _ID);
                    var _productimageUrl = SD.Defaultwwwroot + _productimage.ImageUrl;
                    var _categoryname = _category.Name;
                    var _subcategoryname = _subcategory.SubCategoryName;





                    var productDTO = new AllProductsDTO
                    {
                        Id = _ID,
                        Title = _title,
                        CategoryId = _categoryID,
                        SubCategoryId = _subcategoryID,
                        Category = _categoryname,
                        SubCategory = _subcategoryname,
                        Description = _description,
                        Gender = _gender,
                        Price = _price,
                        ProductImageUrl = _productimageUrl,

                    };
                    productlist.Add(productDTO);


                }


                var maleproductlist = productlist.Where(x=>x.Gender=="Male").ToList();




                _response.Result = maleproductlist;

                _response.StatusCode = HttpStatusCode.OK;
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





        [HttpGet("GetAllFemaleProducts")]
        [ResponseCache(CacheProfileName = "Default30")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetAllFemaleProducts()
        {
            try
            {
                var products = await _unitOfWork.Product.GetAllAsync();

                var productlist = new List<AllProductsDTO>();

                foreach (var product in products)
                {





                    var _ID = product.Id;
                    var _title = product.Title;
                    var _description = product.Description;
                    var _price = product.Price;
                    var _categoryID = product.CategoryId;
                    var _subcategoryID = product.SubCategoryId;
                    var _gender = product.Gender;
                    var _category = await _unitOfWork.Category.GetAsync(u => u.Id == _categoryID);
                    var _subcategory = await _unitOfWork.SubCategory.GetAsync(u => u.Id == _subcategoryID);
                    var _productimage = await _unitOfWork.ProductImage.GetAsync(u => u.ProductId == _ID);
                    var _productimageUrl = SD.Defaultwwwroot + _productimage.ImageUrl;
                    var _categoryname = _category.Name;
                    var _subcategoryname = _subcategory.SubCategoryName;





                    var productDTO = new AllProductsDTO
                    {
                        Id = _ID,
                        Title = _title,
                        CategoryId = _categoryID,
                        SubCategoryId = _subcategoryID,
                        Category = _categoryname,
                        SubCategory = _subcategoryname,
                        Description = _description,
                        Gender = _gender,
                        Price = _price,
                        ProductImageUrl = _productimageUrl,

                    };
                    productlist.Add(productDTO);


                }



                var femaleproductlist = productlist.Where(x => x.Gender == "Female").ToList();



                _response.Result = femaleproductlist;

                _response.StatusCode = HttpStatusCode.OK;
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












        [HttpGet("{id:int}", Name = "GetProduct")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
       
        public async Task<ActionResult<APIResponse>> GetProduct(int id)
        {
            


            try
            {
                if (id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }
                var product = await _unitOfWork.Product.GetAsync(u=>u.Id==id,includeProperties: "Category,SubCategory,ProductImages,Colors,Sizes");

                if (product == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }
                var reviewslist = new List<ReviewDTO>();
                var imageUrls = product.ProductImages.Select(x => x.ImageUrl).ToList();
                var sizes = product.Sizes.Select(x => x.Size).ToList();
                var colors = product.Colors.Select(x => x.Color).ToList();
                var reviewsfromdb = await _Context.Reviews.Where(x=>x.ProductId==id).ToListAsync();
                var reviewscount = reviewsfromdb.Count();
                foreach (var review in reviewsfromdb)
                {
                    var user = await _unitOfWork.ApplicationUser.GetAsync(x=>x.Id==review.ApplicationUserId,includeProperties: "ProfilePicture");

                    var userreviewDTO = new ReviewUserDTO
                    {
                        Id = user.Id,
                        ProfilePicURL = user.ProfilePicture.ImageUrl,
                        FullName = $"{user.FirstName} {user.LastName}"

                    };

                    var reviewDTO = new ReviewDTO
                    {
                        Id = review.Id,
                        ProductId = review.ProductId,
                        User =userreviewDTO,
                        Rate = review.Rate,
                        Comment = review.Comment
                    };
                    reviewslist.Add(reviewDTO);
                }

                var ImageUrlsList = new List<string>();
                foreach (var url in imageUrls) 
                {
                    var item = SD.Defaultwwwroot+url;
                    ImageUrlsList.Add(item);
                
                }




                var productDTO = new ProductDTO
                {
                    Id = id,
                    Title = product.Title,
                    Description = product.Description,
                    Category=product.Category.Name,
                    CategoryId=product.CategoryId,
                    SubCategory=product.SubCategory.SubCategoryName,
                    SubCategoryId=product.SubCategoryId,
                    ProductImagesUrls=ImageUrlsList,
                    Gender=product.Gender,
                    Colors=colors,
                    Sizes=sizes,
                    Price=product.Price,
                    ReviewsCount=reviewscount,
                    Reviews=reviewslist
                   
                };




                _response.Result = productDTO;
                _response.StatusCode = HttpStatusCode.OK;
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

































        [HttpPost("{id:int}", Name = "AddReview")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> AddReview(int id, PostReviewDTO reviewDTO)
        {
            try
            { 
              var claimsIdentity = (ClaimsIdentity)User.Identity;
              var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

              var review = new SnaelyFashion_Models.Review();
              review.Rate = reviewDTO.Rate;
              review.ProductId = id;
              review.ApplicationUserId = userId;
              review.Comment = reviewDTO.Comment;

                _unitOfWork.Review.Add(review);
                _unitOfWork.Save();
                _response.Result = reviewDTO;
                _response.StatusCode = HttpStatusCode.OK;
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







    }
}
