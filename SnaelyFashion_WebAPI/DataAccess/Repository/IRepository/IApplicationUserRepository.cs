using SnaelyFashion_Models;

namespace SnaelyFashion_WebAPI.DataAccess.Repository.IRepository
{
    public interface IApplicationUserRepository : IRepository<ApplicationUser>
    {
        public Task UpdateAsync(ApplicationUser applicationUser);
        public Task DeleteUser (ApplicationUser applicationUser);
    }
}
