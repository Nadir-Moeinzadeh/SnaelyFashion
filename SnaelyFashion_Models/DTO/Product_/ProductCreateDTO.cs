using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using SnaelyFashion_Models.DTO.Review_;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnaelyFashion_Models.DTO.Product_
{
    public class ProductCreateDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }





        public double Price { get; set; }


        public List<string> Size { get; set; }

        public List<string> Colors { get; set; }

        public int CategoryId { get; set; }

        public int SubCategoryId { get; set; }



        [ValidateNever]
        public List<ProductImage>? ProductImages { get; set; }

        public List<ReviewDTO>? Reviews { get; set; }
    }
}
