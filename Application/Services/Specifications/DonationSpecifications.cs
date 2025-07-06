using Domain.Contracts;
using Domain.Models;
using Services.Specifications;

namespace Services.Specifications
{
    public class DonationSpecifications
    {
        public class DonationsByUserSpecification : BaseSpecification<Donation, int>
        {
            public DonationsByUserSpecification(Guid userId) : base(d => d.UserId == userId)
            {
                AddInclude(d => d.Campaign);
                AddOrderByDescending(d => d.DonationDate);
            }
        }

        public class DonationsByCampaignSpecification : BaseSpecification<Donation, int>
        {
            public DonationsByCampaignSpecification(int campaignId) : base(d => d.CampaignId == campaignId)
            {
                AddInclude(d => d.User);
                AddOrderByDescending(d => d.DonationDate);
            }
        }

        public class SuccessfulDonationsSpecification : BaseSpecification<Donation, int>
        {
            public SuccessfulDonationsSpecification() : base(d => d.Status == DonationStatus.Successful)
            {
                AddInclude(d => d.Campaign);
                AddInclude(d => d.User);
                AddOrderByDescending(d => d.DonationDate);
            }
        }

        public class PendingDonationsSpecification : BaseSpecification<Donation, int>
        {
            public PendingDonationsSpecification() : base(d => d.Status == DonationStatus.Pending)
            {
                AddInclude(d => d.Campaign);
                AddInclude(d => d.User);
                AddOrderBy(d => d.DonationDate);
            }
        }

        public class FailedDonationsSpecification : BaseSpecification<Donation, int>
        {
            public FailedDonationsSpecification() : base(d => d.Status == DonationStatus.Failed)
            {
                AddInclude(d => d.Campaign);
                AddInclude(d => d.User);
                AddOrderByDescending(d => d.DonationDate);
            }
        }

        public class DonationWithDetailsSpecification : BaseSpecification<Donation, int>
        {
            public DonationWithDetailsSpecification(int id) : base(d => d.Id == id)
            {
                AddInclude(d => d.Campaign);
                AddInclude(d => d.User);
                AddInclude(d => d.Receipt);
            }
        }

        public class AnonymousDonationsSpecification : BaseSpecification<Donation, int>
        {
            public AnonymousDonationsSpecification() : base(d => d.IsAnonymous == true)
            {
                AddInclude(d => d.Campaign);
                AddOrderByDescending(d => d.DonationDate);
            }
        }
    }
} 