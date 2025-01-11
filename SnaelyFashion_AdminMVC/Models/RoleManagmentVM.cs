using Microsoft.AspNetCore.Mvc.Rendering;
using SnaelyFashion_Models;

namespace SnaelyFashion_AdminMVC.Models
{
    public class RoleManagmentVM
    {
        public ApplicationUser ApplicationUser { get; set; }
        public IEnumerable<SelectListItem> RoleList { get; set; }
    }
}
