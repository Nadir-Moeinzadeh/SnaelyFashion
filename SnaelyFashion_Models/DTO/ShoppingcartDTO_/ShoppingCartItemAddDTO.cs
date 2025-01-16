using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnaelyFashion_Models.DTO.ShoppingcartDTO_
{
    public class ShoppingCartItemAddDTO
    {





        [Range(1, 1000, ErrorMessage = "Please enter a value between 1 and 1000")]
        public int Count { get; set; } = 1;

        

       

        public string Color { get; set; }

        public string Size { get; set; }

       
    }
}
