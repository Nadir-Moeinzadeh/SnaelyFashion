using SnaelyFashion_Models.DTO.ApplicationUser_;

namespace SnaelyFashion_AdminMVC.Services.IServices
{
    public interface IAuthService
    {
        Task<T> LoginAsync<T>(LoginRequestDTO objToCreate);
        Task<T> RegisterAsync<T>(RegisterationRequestDTO objToCreate);
    }
}
