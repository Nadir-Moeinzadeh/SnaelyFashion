using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnaelyFashion_Models.DTO.Product_
{
    public class AllProductsDTO
    {
        public int Id { get; set; }


        public string Title { get; set; }
        public string Description { get; set; }





        public double Price { get; set; }


      

        public int CategoryId { get; set; }

        public string Category { get; set; }



        public int SubCategoryId { get; set; }

        public string SubCategory { get; set; }


        public string ProductImageUrl { get; set; }

        public string? Gender { get; set; }



       
    }
}
