using SnaelyFashion_Models;
using SnaelyFashion_WebAPI.DataAccess.Data;
using SnaelyFashion_WebAPI.DataAccess.Repository.IRepository;

namespace SnaelyFashion_WebAPI.DataAccess.Repository
{
    public class ProfilePictureRepository : Repository<ProfilePicture>, IProfilePictureRepository
    {
        private ApplicationDbContext _db;
        public ProfilePictureRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task UpdateAsync(ProfilePicture obj)
        {
            _db.ProfilePicture.Update(obj);
            await _db.SaveChangesAsync();
        }
    }
}
