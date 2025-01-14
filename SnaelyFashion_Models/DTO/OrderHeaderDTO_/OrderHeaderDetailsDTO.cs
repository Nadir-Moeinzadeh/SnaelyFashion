using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnaelyFashion_Models.DTO.OrderHeaderDTO_
{
    public class OrderHeaderDetailsDTO
    {
       
        public string ApplicationUserId { get; set; }
      
        public double OrderTotal { get; set; }

        
        public string? PhoneNumber { get; set; }
        
        public string? StreetAddress { get; set; }
        
        public string? City { get; set; }
        
        public string? State { get; set; }
        
        public string? PostalCode { get; set; }
        
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
    }
}
