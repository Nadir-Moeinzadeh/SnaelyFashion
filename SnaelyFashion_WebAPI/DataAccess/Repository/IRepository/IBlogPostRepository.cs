using SnaelyFashion_Models;

namespace SnaelyFashion_WebAPI.DataAccess.Repository.IRepository
{
    public interface IBlogPostRepository:IRepository<BlogPost>
    {
        void Update(BlogPost? blogpost);
        Task UpdateAsync(BlogPost obj);
    }
}
