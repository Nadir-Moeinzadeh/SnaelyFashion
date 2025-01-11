using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using SnaelyFashion_Models;

namespace SnaelyFashion_AdminMVC.Models
{
    public class SubCategoryVM
    {
        public SubCategory SubCategory { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> CategoryList { get; set; }
    }
}
