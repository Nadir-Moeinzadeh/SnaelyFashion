using SnaelyFashion_Models;
using SnaelyFashion_WebAPI.DataAccess.Data;
using SnaelyFashion_WebAPI.DataAccess.Repository.IRepository;

namespace SnaelyFashion_WebAPI.DataAccess.Repository
{
    public class ReviewRepository : Repository<Review>, IReviewRepository
    {
        private ApplicationDbContext _db;
        public ReviewRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _db = dbContext;
        }
        public void Update(Review? review)
        {
            _db.Reviews.Update(review);
            _db.SaveChanges();
        }

        public async Task UpdateAsync(Review obj)
        {
            _db.Reviews.Update(obj);
            _db.SaveChangesAsync();
        }
    }
}
