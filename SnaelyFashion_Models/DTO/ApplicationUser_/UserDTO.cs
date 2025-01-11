using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnaelyFashion_Models.DTO.ApplicationUser_
{
    public class UserDTO
    {
        public string ID { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public List <string>? Roles { get; set; }
    }
}
