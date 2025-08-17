using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts
{
    using Domain.Models;
    using System.Linq.Expressions;

    public interface IGenericRepository<T,TKey> where T : BaseEntity<TKey>
    {
        Task<T> GetByIdAsync(TKey id);
        Task<IReadOnlyList<T>> GetAllAsync();

        Task<T> GetEntityWithSpecAsync(ISpecifications<T,TKey> spec);
        Task<IReadOnlyList<T>> ListAsync(ISpecifications<T, TKey> spec);
        Task<int> CountAsync(ISpecifications<T, TKey> spec);

        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        
        // Method for getting queryable (for complex queries)
        IQueryable<T> GetQueryable();
    }

}
