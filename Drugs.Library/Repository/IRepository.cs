using Microsoft.Extensions.DependencyInjection;

namespace Drugs.Library.Repository
{
    public interface IRepository<T> where T : class
    {
        Task AddAsync(T entity);
        Task<IEnumerable<T>> GetAllAsync();
    }
}