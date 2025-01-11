using SnaelyFashion_Models;
using SnaelyFashion_WebAPI.DataAccess.Data;
using SnaelyFashion_WebAPI.DataAccess.Repository.IRepository;

namespace SnaelyFashion_WebAPI.DataAccess.Repository
{
    public class ProductColorRepository : Repository<ProductColor>, IProductColorRepository
    {
        private ApplicationDbContext _db;
        public ProductColorRepository(ApplicationDbContext db):base(db) 
        {
            _db = db;
        }

        public async Task UpdateAsync(ProductColor obj)
        {
            _db.ProductColors.Update(obj);
            await _db.SaveChangesAsync();
        }
    }
}
