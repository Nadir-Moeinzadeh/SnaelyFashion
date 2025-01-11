using AutoMapper.Internal;
using SnaelyFashion_AdminMVC.Models;
using SnaelyFashion_Models;

namespace SnaelyFashion_AdminMVC.Services.IServices
{
    public interface IBaseService
    {
        APIResponse responseModel { get; set; }
        Task<T> SendAsync<T>(APIRequest apiRequest);
    }
}
