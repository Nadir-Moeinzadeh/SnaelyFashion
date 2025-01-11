using SnaelyFashion_Models;
using SnaelyFashion_WebAPI.DataAccess.Data;
using SnaelyFashion_WebAPI.DataAccess.Repository.IRepository;

namespace SnaelyFashion_WebAPI.DataAccess.Repository
{
    public class BlogPostRepository : Repository<BlogPost>, IBlogPostRepository
    {
        private ApplicationDbContext _db;
        public BlogPostRepository(ApplicationDbContext dbContext):base(dbContext) 
        {
             _db = dbContext;
        }
        public void Update(BlogPost? blogpost)
        {
           _db.BlogPosts.Update(blogpost);
            _db.SaveChanges();
        }

        public async Task UpdateAsync(BlogPost obj)
        {
             _db.BlogPosts.Update(obj);
            _db.SaveChangesAsync();
        }
    }
}
