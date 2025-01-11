using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnaelyFashion_Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        
        public string Title { get; set; }
        public string Description { get; set; }
        [Required]


        [Display(Name = "Price")]

        public double Price { get; set; }

        [Display(Name = "Size")]
        public List<ProductSize> Sizes { get; set; }
        [Display(Name = "Colors")]
        public List<ProductColor> Colors { get; set; }

        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        [ValidateNever]
        public Category Category { get; set; }

        [ValidateNever]
        
        public int SubCategoryId { get; set; }
        [ForeignKey("SubCategoryId")]
        [ValidateNever]
        public SubCategory SubCategory { get; set; }

        [ValidateNever]
        public List<ProductImage>? ProductImages { get; set; }

        public string? Gender { get; set; }

      

        public List<Review>? Reviews { get; set; }
    }
}
