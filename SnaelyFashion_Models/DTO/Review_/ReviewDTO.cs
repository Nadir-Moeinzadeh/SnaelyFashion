using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnaelyFashion_Models.DTO.Review_
{
    public class ReviewDTO
    {
        public int Id { get; set; }

      
        public double Rate { get; set; }

        public string Comment { get; set; }

      
        public ReviewUserDTO User { get; set; }

        public int ProductId { get; set; }
        
      

    }
}
