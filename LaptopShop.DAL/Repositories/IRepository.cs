using System.Linq.Expressions;

namespace LaptopShop.DAL.Repositories;

public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync(
        Expression<Func<T, bool>> filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
        string includeProperties = "");
    Task<T> GetByIdAsync(object id);
    Task InsertAsync(T entity);
    Task DeleteAsync(object id);
    void Delete(T entityToDelete);
    void Update(T entityToUpdate);
    Task SaveAsync();
}
