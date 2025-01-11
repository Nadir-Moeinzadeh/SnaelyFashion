using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using SnaelyFashion_Models;

namespace SnaelyFashion_Models.DTO.Category_
{
    public class CategoryDTO
    {
        public int Id { get; set; }
       
        public string Name { get; set; }
        
        

        public List<SubCategory>? SubCategories { get; set; }

        public List<Product>? Products { get; set; }

    }
}
