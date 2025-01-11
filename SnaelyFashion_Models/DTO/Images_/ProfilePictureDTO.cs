using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnaelyFashion_Models.DTO.Images_
{
    public class ProfilePictureDTO
    {
        public string Id { get; set; }
        
        public string ImageUrl { get; set; }
        public string ApplicationUserId { get; set; }
    }
}
