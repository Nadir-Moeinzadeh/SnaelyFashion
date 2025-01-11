using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnaelyFashion_Models
{
    public class BlogPostImage
    {
        public int Id { get; set; }
        [Required]
        public string ImageUrl { get; set; }
        public int BlogPostId { get; set; }
        [ForeignKey("BlogPostId")]
        public BlogPost BlogPost { get; set; }
    }
}
