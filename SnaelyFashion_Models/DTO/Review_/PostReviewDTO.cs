using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnaelyFashion_Models.DTO.Review_
{
    public class PostReviewDTO
    {
       


        public double Rate { get; set; }

        public string Comment { get; set; }

        public int ProductId { get; set; }
    }
}
