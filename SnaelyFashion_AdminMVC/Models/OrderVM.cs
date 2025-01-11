using SnaelyFashion_Models;

namespace SnaelyFashion_AdminMVC.Models
{
    public class OrderVM
    {
        public OrderHeader OrderHeader { get; set; }
        public IEnumerable<OrderDetail> OrderDetail { get; set; }
    }
}
