using AutoMapper;
using SnaelyFashion_Models.DTO.ApplicationUser_;
using SnaelyFashion_Models.DTO.Category_;
using SnaelyFashion_Models;
using SnaelyFashion_Models.DTO.Product_;

namespace SnaelyFashion_WebAPI.Configurations
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<ApplicationUser, UserDTO>().ReverseMap();
            CreateMap<Category, CategoryDTO>().ReverseMap();
            CreateMap<Category,CategoryCreateDTO>().ReverseMap();
            CreateMap<Product,ProductDTO>().ReverseMap();
        }
    }
}
