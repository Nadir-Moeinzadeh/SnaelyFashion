using SnaelyFashion_Models;
using SnaelyFashion_WebAPI.DataAccess.Data;
using SnaelyFashion_WebAPI.DataAccess.Repository.IRepository;

namespace SnaelyFashion_WebAPI.DataAccess.Repository
{
    public class ProductSizeRepository : Repository<ProductSize>, IProductSizeRepository
    {
        private ApplicationDbContext _db;
        public ProductSizeRepository(ApplicationDbContext db):base(db) 
        {
            _db = db;
        }
        public async Task UpdateAsync(ProductSize obj)
        {
           _db.ProductSizes.Update(obj);
            await _db.SaveChangesAsync();
        }
    }
}
