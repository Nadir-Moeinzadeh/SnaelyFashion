﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnaelyFashion_Models.DTO.ApplicationUser_
{
    public class RegisterationRequestDTO
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        //public string PasswordConfirmation { get; set; }

        //public string? PhoneNumber { get; set; }
        //public string? ImageUrl { get; set; }
        //public string? StreetAddress { get; set; }
        //public string? City { get; set; }
        //public string? State { get; set; }
        //public string? PostalCode { get; set; }
        //public string Role { get; set; }
    }
}
