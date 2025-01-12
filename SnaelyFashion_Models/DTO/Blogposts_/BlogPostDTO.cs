using SnaelyFashion_Models.DTO.Review_;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnaelyFashion_Models.DTO.Blogposts_
{
    public class BlogPostDTO
    {
        public int Id { get; set; }


        public string Title { get; set; }
        public string Description { get; set; }

        public List<string>? BlogPostImagesUrls { get; set; }

       
    }
}
