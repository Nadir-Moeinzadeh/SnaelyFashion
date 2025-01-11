using SnaelyFashion_Models;

namespace SnaelyFashion_WebAPI.DataAccess.Repository.IRepository
{
    public interface ISubCategoryRepository:IRepository<SubCategory>
    {
        Task UpdateAsync(SubCategory obj);
    }
}
