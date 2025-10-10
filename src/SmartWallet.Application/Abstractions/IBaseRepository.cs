using System.Linq.Expressions;

namespace Application.Abstraction;

public interface IBaseRepository<T> where T : class
{
    List<T> GetAll();
    T? GetById(Guid id);
    bool Create(T entity);
    bool Update(T entity);
    bool Delete(T entity);
    List<T> GetByCriteria(Expression<Func<T, bool>> expression);
}
