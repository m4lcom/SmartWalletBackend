using System.Linq.Expressions;


namespace SmartWallet.Application.Abstractions
{
    public interface IBaseRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync();
        Task<T?> GetByIdAsync(Guid id);
        Task<bool> CreateAsync(T entity);
        Task<bool> UpdateAsync(T entity);
        Task<bool> DeleteAsync(T entity);
        Task<List<T>> GetByCriteriaAsync(Expression<Func<T, bool>> expression);
    }
}
