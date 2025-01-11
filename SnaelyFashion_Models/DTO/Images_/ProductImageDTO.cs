using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnaelyFashion_Models.DTO.Images_
{
    public class ProductImageDTO
    {
        public int Id { get; set; }
       
        public string ImageUrl { get; set; }
        public int ProductId { get; set; }
    }
}
