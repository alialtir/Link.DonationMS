using Domain.Contracts;
using Domain.Models;
using Services.Specifications;

namespace Services.Specifications
{
    public class ReceiptSpecifications
    {
        public class ReceiptByDonationSpecification : BaseSpecification<Receipt, int>
        {
            public ReceiptByDonationSpecification(int donationId) : base(r => r.DonationId == donationId)
            {
                AddInclude(r => r.Donation);
                AddInclude(r => r.Donation.Campaign);
                AddInclude(r => r.Donation.User);
            }
        }

        public class ReceiptsByUserSpecification : BaseSpecification<Receipt, int>
        {
            public ReceiptsByUserSpecification(Guid userId) : base(r => r.Donation.UserId == userId)
            {
                AddInclude(r => r.Donation);
                AddInclude(r => r.Donation.Campaign);
                AddOrderByDescending(r => r.Donation.DonationDate);
            }
        }

        public class ReceiptWithDetailsSpecification : BaseSpecification<Receipt, int>
        {
            public ReceiptWithDetailsSpecification(int id) : base(r => r.Id == id)
            {
                AddInclude(r => r.Donation);
                AddInclude(r => r.Donation.Campaign);
                AddInclude(r => r.Donation.User);
            }
        }
    }
} 