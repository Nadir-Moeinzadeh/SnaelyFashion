using SnaelyFashion_Models.DTO.OrderHeaderDTO_;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnaelyFashion_Models.DTO.ShoppingcartDTO_
{
    public class ShoppingCartDTO
    {
        public IEnumerable<ShoppingCartItemDTO> ShoppingCartList { get; set; }
        public OrderHeaderDetailsDTO OrderHeaderDetails { get; set; }
        public int? OrderHeaderId { get; set; }
    }
}
