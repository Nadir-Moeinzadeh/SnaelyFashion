using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnaelyFashion_Models.DTO.Profile_
{
    public class ProfileDTO
    {
        public string ID { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        
        public string LastName { get; set; }

        public string FullName { get; set; }
        public string? ProfilePictureURL { get; set; }
        public string? PhoneNumber { get; set; }
        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
    }
}
