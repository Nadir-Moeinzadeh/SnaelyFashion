using SnaelyFashion_AdminMVC.Models;
using SnaelyFashion_AdminMVC.Services.IServices;
using SnaelyFashion_Models.DTO.ApplicationUser_;
using SnaelyFashion_Utility;

namespace SnaelyFashion_AdminMVC.Services
{
    public class AuthService : BaseService, IAuthService
    {
        private readonly IHttpClientFactory _clientFactory;
        private string SnaelyUrl;

        public AuthService(IHttpClientFactory clientFactory, IConfiguration configuration) : base(clientFactory)
        {
            _clientFactory = clientFactory;
            SnaelyUrl = configuration.GetValue<string>("ServiceUrls:WebAPI");

        }

        public Task<T> LoginAsync<T>(LoginRequestDTO obj)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = obj,
                Url = SnaelyUrl + "/api/Auth/login"
            });
        }

        public Task<T> RegisterAsync<T>(RegisterationRequestDTO obj)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = obj,
                Url = SnaelyUrl + "/api/Auth/register"
            });
        }
    }
}
