using System.Linq.Expressions;

namespace SnaelyFashion_WebAPI.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {

        Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null);
        Task<T> GetAsync(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false);

        
        Task CreateAsync(T entity);
        void Remove(T entity);
        Task RemoveRangeAsync(IEnumerable<T> entity);
        Task SaveAsync();

        Task AddRangeAsync(IEnumerable<T> entity);

        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null);
        T Get(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false);
        void Add(T entity);
        void AddRange(IEnumerable<T> entity);
        void RemoveRange(IEnumerable<T> entity);
    }
}
