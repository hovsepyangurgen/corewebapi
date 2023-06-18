using System.Linq.Expressions;

namespace RestMng.Core
{
    public interface IRepository<T> : IDisposable where T : class, IEntity
    {
        Task<List<T>> GetAll();
        Task<T> Get(int id);
        Task<T> Add(T entity);
        Task<T> Update(T entity);
        Task<T> Delete(int id);
        Task<T> Set(T entity);
        Task<T> GetByKey(Expression<Func<T, bool>> predicate);
        Task AddRange(IEnumerable<T> items);
        Task DeleteAll();
    }
}
