using SnaelyFashion_Models;

namespace SnaelyFashion_WebAPI.DataAccess.Repository.IRepository
{
    public interface IProfilePictureRepository : IRepository<ProfilePicture>
    {
        Task UpdateAsync(ProfilePicture obj);
    }
}
