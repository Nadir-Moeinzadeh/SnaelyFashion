using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnaelyFashion_Models.DTO.Blogposts_
{
    public class GetAllBlogPostsDTO
    {
        public int Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public string BlogpostImageUrl { get; set; }

        public DateTime CreatedDate { get; set; }


    }
}
