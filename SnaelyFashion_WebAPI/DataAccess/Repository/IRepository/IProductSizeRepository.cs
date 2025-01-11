using SnaelyFashion_Models;

namespace SnaelyFashion_WebAPI.DataAccess.Repository.IRepository
{
    public interface IProductSizeRepository : IRepository<ProductSize>
    {
        public Task UpdateAsync(ProductSize obj);
    }
}
