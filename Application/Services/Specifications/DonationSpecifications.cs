using Domain.Contracts;
using Domain.Models;
using Services.Specifications;

namespace Services.Specifications
{
    public class DonationSpecifications
    {
        public class DonationsByUserSpecification : BaseSpecification<Donation, Guid>
        {
            public DonationsByUserSpecification(Guid userId) : base(d => d.UserId == userId)
            {
                AddInclude(d => d.Campaign);
                AddOrderByDescending(d => d.DonationDate);
            }
        }

        public class DonationsByCampaignSpecification : BaseSpecification<Donation, Guid>
        {
            public DonationsByCampaignSpecification(int campaignId) : base(d => d.CampaignId == campaignId)
            {
                AddInclude(d => d.User);
                AddOrderByDescending(d => d.DonationDate);
            }
        }

        public class SuccessfulDonationsSpecification : BaseSpecification<Donation, Guid>
        {
            public SuccessfulDonationsSpecification() : base(d => d.Status == DonationStatus.Successful)
            {
                AddInclude(d => d.Campaign);
                AddInclude(d => d.User);
                AddOrderByDescending(d => d.DonationDate);
            }
        }

        public class SuccessfulDonationsByCampaignSpecification : BaseSpecification<Donation, Guid>
        {
            public SuccessfulDonationsByCampaignSpecification(int campaignId) : base(d => 
                d.CampaignId == campaignId && d.Status == DonationStatus.Successful)
            {
                
            }
        }

        public class PendingDonationsSpecification : BaseSpecification<Donation, Guid>
        {
            public PendingDonationsSpecification() : base(d => d.Status == DonationStatus.Pending)
            {
                AddInclude(d => d.Campaign);
                AddInclude(d => d.User);
                AddOrderBy(d => d.DonationDate);
            }
        }

        public class FailedDonationsSpecification : BaseSpecification<Donation, Guid>
        {
            public FailedDonationsSpecification() : base(d => d.Status == DonationStatus.Failed)
            {
                AddInclude(d => d.Campaign);
                AddInclude(d => d.User);
                AddOrderByDescending(d => d.DonationDate);
            }
        }

        public class DonationWithDetailsSpecification : BaseSpecification<Donation, Guid>
        {
            public DonationWithDetailsSpecification(Guid id) : base(d => d.Id == id)
            {
                AddInclude(d => d.Campaign);
                AddInclude(d => d.User);
                AddInclude(d => d.Receipt);
            }
        }

        public class AnonymousDonationsSpecification : BaseSpecification<Donation, Guid>
        {
            public AnonymousDonationsSpecification() : base(d => d.IsAnonymous == true)
            {
                AddInclude(d => d.Campaign);
                AddOrderByDescending(d => d.DonationDate);
            }
        }

        public class DonationByStripeIdsSpecification : BaseSpecification<Donation, Guid>
        {
            public DonationByStripeIdsSpecification(string sessionId, string paymentIntentId) : base(d =>
                (sessionId != null && d.PaymentId == sessionId) ||
                (paymentIntentId != null && d.PaymentId == paymentIntentId))
            {
                AddInclude(d => d.Campaign);
                AddInclude(d => d.User);
            }
        }

        public class DonationsByCampaignWithUserSpecification : BaseSpecification<Donation, Guid>
        {
            public DonationsByCampaignWithUserSpecification(int campaignId) : base(d => d.CampaignId == campaignId)
            {
                AddInclude(d => d.User);
            }
        }

        public class RecentRandomDonationsWithUserSpecification : BaseSpecification<Donation, Guid>
        {
            public RecentRandomDonationsWithUserSpecification(int count) : base()
            {
                AddInclude(d => d.User);
                // Order by NEWID() for random ordering in SQL Server
                AddOrderBy(d => Guid.NewGuid());
                ApplyPagination(1, count);
            }

            public RecentRandomDonationsWithUserSpecification(int campaignId, int count) : base(d => d.CampaignId == campaignId)
            {
                AddInclude(d => d.User);
                AddOrderBy(d => Guid.NewGuid());
                ApplyPagination(1, count);
            }
        }

        public class DonationsWithUserSpecification : BaseSpecification<Donation, Guid>
        {
            public DonationsWithUserSpecification() : base()
            {
                AddInclude(d => d.User);
            }
        }

        public class DonationsWithPaginationSpecification : BaseSpecification<Donation, Guid>
        {
            public DonationsWithPaginationSpecification(int pageNumber, int pageSize) : base()
            {
                AddInclude(d => d.Campaign);
                AddInclude(d => d.User);
                ApplyPagination(pageNumber, pageSize);
            }
        }
    }
} 