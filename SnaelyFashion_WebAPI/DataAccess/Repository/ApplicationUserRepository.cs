using SnaelyFashion_Models;
using SnaelyFashion_WebAPI.DataAccess.Data;
using SnaelyFashion_WebAPI.DataAccess.Repository.IRepository;

namespace SnaelyFashion_WebAPI.DataAccess.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private ApplicationDbContext _db;
        public ApplicationUserRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task DeleteUser(ApplicationUser applicationUser)
        {
            _db.ApplicationUsers.Remove(applicationUser);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(ApplicationUser applicationUser)
        {
            _db.ApplicationUsers.Update(applicationUser);
            await _db.SaveChangesAsync();
        }



    }
}
