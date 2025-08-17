using Domain.Contracts;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class GenericRepository<T, TKey> : IGenericRepository<T, TKey> where T : BaseEntity<TKey>
    {
        protected readonly DonationDbContext _context;
        public GenericRepository(DonationDbContext context) => _context = context;

        public async Task<T> GetByIdAsync(TKey id) => await _context.Set<T>().FindAsync(id);

        public async Task<IReadOnlyList<T>> GetAllAsync() => await _context.Set<T>().ToListAsync();

        public async Task<T> GetEntityWithSpecAsync(ISpecifications<T, TKey> spec) =>
            await ApplySpecification(spec).FirstOrDefaultAsync();

        public async Task<IReadOnlyList<T>> ListAsync(ISpecifications<T, TKey> spec) =>
            await ApplySpecification(spec).ToListAsync();

        public async Task<int> CountAsync(ISpecifications<T, TKey> spec) =>
            await ApplySpecification(spec).CountAsync();

        public async Task AddAsync(T entity) => await _context.Set<T>().AddAsync(entity);

        public void Update(T entity) => _context.Set<T>().Update(entity);

        public void Delete(T entity) => _context.Set<T>().Remove(entity);

        public IQueryable<T> GetQueryable() => _context.Set<T>().AsQueryable();

        private IQueryable<T> ApplySpecification(ISpecifications<T, TKey> spec)
            => SpecificationEvaluator<T, TKey>.GetQuery(_context.Set<T>().AsQueryable(), spec);
    }


}
