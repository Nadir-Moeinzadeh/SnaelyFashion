using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SnaelyFashion_Models.DTO.Images_;
using SnaelyFashion_Models.DTO.Review_;

namespace SnaelyFashion_Models.DTO.Product_
{
    public class ProductDTO
    {
        
        public int Id { get; set; }
       

        public string Title { get; set; }
        public string Description { get; set; }
       


        

        public double Price { get; set; }

        
        public List<string>? Sizes { get; set; }
       
        public List<string>? Colors { get; set; }

        public int CategoryId { get; set; }
       
        public string Category { get; set; }

       

        public int SubCategoryId { get; set; }
        
        public string SubCategory { get; set; }

       
        public List<string>? ProductImagesUrls { get; set; }

        public string? Gender { get; set; }



        public List<ReviewDTO>? Reviews { get; set; }

        public int ReviewsCount { get; set; }   
    }
}
