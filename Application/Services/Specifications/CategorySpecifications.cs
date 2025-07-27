using Domain.Contracts;
using Domain.Models;
using Services.Specifications;
using System.Linq;

namespace Services.Specifications
{
    public class CategorySpecifications
    {
        public class CategoryWithCampaignsSpecification : BaseSpecification<Category, int>
        {
            public CategoryWithCampaignsSpecification(int id) : base(c => c.Id == id)
            {
                AddInclude(c => c.Campaigns);
                AddOrderBy(c => c.TitleAr);
            }
        }

        public class ActiveCategoriesSpecification : BaseSpecification<Category, int>
        {
            public ActiveCategoriesSpecification() : base(c => c.Campaigns.Count > 0)
            {
                AddInclude(c => c.Campaigns);
                AddOrderBy(c => c.TitleAr);
            }
        }

        public class CategoryByTitleSpecification : BaseSpecification<Category, int>
        {
            public CategoryByTitleSpecification(string searchTerm) : base(c => 
                c.TitleAr.Contains(searchTerm) || c.TitleEn.Contains(searchTerm))
            {
                AddInclude(c => c.Campaigns);
                AddOrderBy(c => c.TitleAr);
            }
        }

        public class CategoriesWithPaginationSpecification : BaseSpecification<Category, int>
        {
            public CategoriesWithPaginationSpecification(int pageNumber, int pageSize) : base()
            {
                AddOrderBy(c => c.TitleAr);
                ApplyPagination(pageNumber, pageSize);
            }
        }

        public class AllCategoriesSpecification : BaseSpecification<Category, int>
        {
            public AllCategoriesSpecification() : base()
            {
                AddOrderBy(c => c.TitleAr);
            }
        }
    }
} 