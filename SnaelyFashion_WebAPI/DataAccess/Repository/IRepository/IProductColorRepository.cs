using SnaelyFashion_Models;

namespace SnaelyFashion_WebAPI.DataAccess.Repository.IRepository
{
    public interface IProductColorRepository : IRepository<ProductColor>
    {
        public Task UpdateAsync(ProductColor obj);
    }
}
