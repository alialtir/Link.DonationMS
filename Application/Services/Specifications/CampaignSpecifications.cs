using Domain.Contracts;
using Domain.Models;
using Services.Specifications;

namespace Services.Specifications
{
    public class CampaignSpecifications
    {
        public class ActiveCampaignsSpecification : BaseSpecification<Campaign, int>
        {
            public ActiveCampaignsSpecification() : base(c => c.Status == CampaignStatus.Active)
            {
                AddInclude(c => c.Category);
                AddOrderBy(c => c.EndDate);
            }
        }

        public class CompletedCampaignsSpecification : BaseSpecification<Campaign, int>
        {
            public CompletedCampaignsSpecification() : base(c => c.Status == CampaignStatus.Completed)
            {
                AddInclude(c => c.Category);
                AddOrderByDescending(c => c.EndDate);
            }
        }

        public class CampaignsByCategorySpecification : BaseSpecification<Campaign, int>
        {
            public CampaignsByCategorySpecification(int categoryId) : base(c => c.CategoryId == categoryId)
            {
                AddInclude(c => c.Category);
                AddOrderBy(c => c.EndDate);
            }
        }

        public class CampaignsByTitleSpecification : BaseSpecification<Campaign, int>
        {
            public CampaignsByTitleSpecification(string searchTerm) : base(c => 
                c.TitleAr.Contains(searchTerm) || c.TitleEn.Contains(searchTerm))
            {
                AddInclude(c => c.Category);
                AddOrderBy(c => c.EndDate);
            }
        }

        public class CampaignWithDetailsSpecification : BaseSpecification<Campaign, int>
        {
            public CampaignWithDetailsSpecification(int id) : base(c => c.Id == id)
            {
                AddInclude(c => c.Category);
                AddInclude(c => c.Donations);
            }
        }

        public class TopCampaignsSpecification : BaseSpecification<Campaign, int>
        {
            public TopCampaignsSpecification(int count) : base(c => c.Status == CampaignStatus.Active)
            {
                AddInclude(c => c.Category);
                AddOrderByDescending(c => c.CurrentAmount);
                ApplyPagination(1, count);
            }
        }

        public class CampaignsWithPaginationSpecification : BaseSpecification<Campaign, int>
        {
            public CampaignsWithPaginationSpecification(int pageNumber, int pageSize) : base()
            {
                AddInclude(c => c.Category);
                ApplyPagination(pageNumber, pageSize);
            }
        }

        public class AllCampaignsSpecification : BaseSpecification<Campaign, int>
        {
            public AllCampaignsSpecification() : base()
            {
            }
        }

        public class ActiveCampaignsFilteredSpecification : BaseSpecification<Campaign, int>
        {
            public ActiveCampaignsFilteredSpecification(string title = null, int? categoryId = null)
                : base(c => c.Status == CampaignStatus.Active &&
                    (string.IsNullOrEmpty(title) || c.TitleAr.Contains(title) || c.TitleEn.Contains(title)) &&
                    (!categoryId.HasValue || c.CategoryId == categoryId.Value))
            {
                AddInclude(c => c.Category);
                AddOrderBy(c => c.EndDate);
            }
        }
    }
} 