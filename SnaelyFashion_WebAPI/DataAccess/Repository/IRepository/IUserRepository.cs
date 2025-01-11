using SnaelyFashion_Models.DTO.ApplicationUser_;
using SnaelyFashion_Models;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace SnaelyFashion_WebAPI.DataAccess.Repository.IRepository
{
    public interface IUserRepository
    {
        public Task UpdateAsync(ApplicationUser applicationUser);

        bool IsUniqueUser(string username);
        Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO);
        Task<UserDTO> Register(RegisterationRequestDTO registerationRequestDTO);

        Task SignOut();
        Task SaveAsync();
    }
}
