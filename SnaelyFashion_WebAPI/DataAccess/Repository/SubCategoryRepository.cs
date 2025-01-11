using SnaelyFashion_Models;
using SnaelyFashion_WebAPI.DataAccess.Data;
using SnaelyFashion_WebAPI.DataAccess.Repository.IRepository;

namespace SnaelyFashion_WebAPI.DataAccess.Repository
{
    public class SubCategoryRepository : Repository<SubCategory>, ISubCategoryRepository
    {
        private ApplicationDbContext _db;
        public SubCategoryRepository(ApplicationDbContext db):base(db) 
        {
        _db = db;
        }
        public async Task UpdateAsync(SubCategory entity)
        {

            _db.SubCategories.Update(entity);
            await _db.SaveChangesAsync();

        }
    }
    
}
