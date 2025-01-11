using SnaelyFashion_Models;

namespace SnaelyFashion_WebAPI.DataAccess.Repository.IRepository
{
    public interface IProductRepository : IRepository<Product>
    {
        void Update(Product? product);
        Task UpdateAsync(Product obj);
    }
}
