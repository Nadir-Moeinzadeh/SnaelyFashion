using SnaelyFashion_Models;

namespace SnaelyFashion_WebAPI.DataAccess.Repository.IRepository
{
    public interface IBlogPostImageRepository:IRepository<BlogPostImage>
    {
        void Update(BlogPostImage? blogpostimage);
        Task UpdateAsync(BlogPostImage obj);
    }
}
