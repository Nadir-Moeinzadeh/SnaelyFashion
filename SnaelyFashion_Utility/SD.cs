using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnaelyFashion_Utility
{
    public static class SD
    {
        public const string Role_Customer = "Customer";
        public const string Role_Admin = "Admin";
        public const string Role_Employee = "Employee";

        public const string StatusPending = "Pending";
        public const string StatusApproved = "Approved";
        public const string StatusInProcess = "Processing";
        public const string StatusShipped = "Shipped";
        public const string StatusCancelled = "Cancelled";
        public const string StatusRefunded = "Refunded";

        public const string PaymentStatusPending = "Pending";
        public const string PaymentStatusApproved = "Approved";
        public const string PaymentStatusDelayedPayment = "ApprovedForDelayedPayment";
        public const string PaymentStatusRejected = "Rejected";


        public const string SessionCart = "SessionShoppingCart";


       


        public enum ApiType
        {
            GET,
            POST,
            PUT,
            DELETE
        }
        public static string SessionToken = "JWTToken";

        public static readonly IEnumerable<string> Genders = new List<string> { "Male", "Female" };

        public static readonly IEnumerable<string> Sizes = new List<string> { "S", "M","L","XS","XL","XXL","3XL","4XL","5XL"};
        public static readonly IEnumerable<string> ShoeSizes = new List<string> { "35", "36","37","38","39","40","41","42","43", "44", "45", "46","47" };
        public static readonly IEnumerable<string> Colors = new List<string> { "Black", "White","Grey", "Purple", "Red", "Brown", "Blue", "Yellow", "Orange", "Pink", "Green", "Wine Red", "Leaf Green", "Cream" };
        public static readonly IEnumerable<string> PantsSizes = new List<string> { "26", "28", "30", "32", "34", "36", "38", "40", "42", "44", "46", "48", "50", "52" ,"54","56","58","60"};
    }
}
