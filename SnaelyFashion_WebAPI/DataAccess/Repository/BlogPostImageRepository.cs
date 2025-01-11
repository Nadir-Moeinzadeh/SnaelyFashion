using SnaelyFashion_Models;
using SnaelyFashion_WebAPI.DataAccess.Data;
using SnaelyFashion_WebAPI.DataAccess.Repository.IRepository;

namespace SnaelyFashion_WebAPI.DataAccess.Repository
{
    public class BlogPostImageRepository : Repository<BlogPostImage>, IBlogPostImageRepository
    {
        private readonly ApplicationDbContext _db;
        public BlogPostImageRepository(ApplicationDbContext dbContext):base(dbContext)
        {
            _db = dbContext;
        }

        public void Update(BlogPostImage? blogpostimage)
        {
           _db.BlogPostImages.Update(blogpostimage);
            _db.SaveChanges();
        }

        public async Task UpdateAsync(BlogPostImage obj)
        {
            _db.BlogPostImages.Update(obj);
           await _db.SaveChangesAsync();
        }
    }
}
