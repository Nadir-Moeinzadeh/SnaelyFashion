using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SnaelyFashion_Models.DTO.Blogposts_;
using SnaelyFashion_Models;
using System.Net;
using AutoMapper;
using SnaelyFashion_WebAPI.DataAccess.Data;
using SnaelyFashion_WebAPI.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using SnaelyFashion_Models.DTO.Product_;
using SnaelyFashion_Models.DTO.Review_;

namespace SnaelyFashion_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogsController : ControllerBase
    {
        protected APIResponse _response;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _Context;
        public BlogsController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment, ApplicationDbContext dbContext, IMapper mapper)
        {
            
            _unitOfWork = unitOfWork;   
            _webHostEnvironment = webHostEnvironment;
            _mapper = mapper;
            _Context = dbContext;
            _response= new APIResponse();

        }










        [HttpGet("GetAllBlogposts")]
        [ResponseCache(CacheProfileName = "Default30")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetAllBlogposts()
        {
            try
            {
                IEnumerable<BlogPost> blogpostList = _unitOfWork.BlogPost.GetAll(includeProperties: "blogPostImages").OrderBy(x => x.CreatedDate).Reverse();
                

                var blogpostDTOlist = new List<GetAllBlogPostsDTO>();

                foreach (var blogpost in blogpostList)
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
                        BlogpostImageUrl = _blogpostimageUrl,
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


        [HttpGet("{id:int}", Name = "GetBlogPost")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<APIResponse>> GetBlogPost(int id)
        {



            try
            {
                if (id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }
                var blogpost = await _unitOfWork.BlogPost.GetAsync(u => u.Id == id, includeProperties: "blogPostImages");

                if (blogpost == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }
               
                var imageUrls = blogpost.blogPostImages.Select(x => x.ImageUrl).ToList();
                
              





                var blogpostDTO = new BlogPostDTO
                {
                    Id = id,
                    Title = blogpost.Title,
                    Description = blogpost.Description,
                   
                    BlogPostImagesUrls = imageUrls
                   

                };




                _response.Result = blogpostDTO;
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
