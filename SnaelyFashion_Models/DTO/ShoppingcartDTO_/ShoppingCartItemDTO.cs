using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnaelyFashion_Models.DTO.ShoppingcartDTO_
{
    public class ShoppingCartItemDTO
    {

        public int? ShoppingCartId { get; set; }
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        [Range(1, 1000, ErrorMessage = "Please enter a value between 1 and 1000")]
        public int Count { get; set; }

        public string ApplicationUserId { get; set; }

        [NotMapped]
        public double Price { get; set; }

        public string Color { get; set; }

        public string Size { get; set; }

        [NotMapped]
        public string? ImageUrl { get; set; }
    }
}
