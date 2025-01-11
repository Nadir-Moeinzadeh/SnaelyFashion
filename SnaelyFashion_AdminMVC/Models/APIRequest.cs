using SnaelyFashion_Utility;
using static SnaelyFashion_Utility.SD;

namespace SnaelyFashion_AdminMVC.Models
{
    public class APIRequest
    {
        public ApiType ApiType { get; set; } = ApiType.GET;
        public string Url { get; set; }
        public object Data { get; set; }
        public string Token { get; set; }
    }
}
