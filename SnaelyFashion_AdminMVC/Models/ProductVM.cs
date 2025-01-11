using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using SnaelyFashion_Models;

namespace SnaelyFashion_AdminMVC.Models
{
    public class ProductVM
    {
        public Product? Product { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem>? CategoryList { get; set; }
        public IEnumerable<SelectListItem>? SubCategoryList { get; set; }
        public IEnumerable<SelectListItem>? Gender { get; set; }

        public List<CheckboxOption>? SizeCheckBoxes { get; set; }
        public List<CheckboxOption>? ShoeSizeCheckBoxes { get; set; }

      
        public List<CheckboxOption>? ColorsCheckBoxes { get; set; }

        public List<string>? SelectedSizes { get; set; }
        public List<string>? SelectedColors { get; set; }

    }
}
