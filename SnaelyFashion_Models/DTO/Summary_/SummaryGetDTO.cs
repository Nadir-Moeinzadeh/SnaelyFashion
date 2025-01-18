using SnaelyFashion_Models.DTO.OrderHeaderDTO_;
using SnaelyFashion_Models.DTO.ShoppingcartDTO_;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnaelyFashion_Models.DTO.Summary_
{
    public class SummaryGetDTO
    {
        public IEnumerable<ShoppingCartItemDTO> ShoppingCartList { get; set; }
        public OrderHeaderDetailsDTO OrderHeaderDetails { get; set; }
    }
}
