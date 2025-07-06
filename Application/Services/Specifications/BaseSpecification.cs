using Domain.Contracts;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Services.Specifications
{
    public class BaseSpecification<T, TKey> : ISpecifications<T, TKey> where T : BaseEntity<TKey>
    {
        public Expression<Func<T, bool>> Criteria { get; private set; }
        public List<Expression<Func<T, object>>> Includes { get; } = new();
        public Expression<Func<T, object>> OrderBy { get; private set; }
        public Expression<Func<T, object>> OrderByDescending { get; private set; }
        public int Take { get; private set; }
        public int Skip { get; private set; }
        public bool IsPagingEnabled { get; private set; }

        public BaseSpecification() { }

        public BaseSpecification(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }

        protected void AddInclude(Expression<Func<T, object>> include) => Includes.Add(include);

        protected void AddOrderBy(Expression<Func<T, object>> orderBy) => OrderBy = orderBy;

        protected void AddOrderByDescending(Expression<Func<T, object>> orderByDesc) => OrderByDescending = orderByDesc;

        protected void ApplyPagination(int pageIndex, int pageSize)
        {
            IsPagingEnabled = true;
            Take = pageSize;
            Skip = pageSize * (pageIndex - 1);
        }
    }
}
