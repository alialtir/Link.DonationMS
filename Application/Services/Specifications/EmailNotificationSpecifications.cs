using Domain.Contracts;
using Domain.Models;
using Services.Specifications;

namespace Services.Specifications
{
    public class EmailNotificationSpecifications
    {
        public class NotificationsByUserSpecification : BaseSpecification<EmailNotification, int>
        {
            public NotificationsByUserSpecification(Guid userId) : base(n => n.UserId == userId)
            {
                AddInclude(n => n.Donation);
                AddInclude(n => n.Donation.Campaign);
                AddOrderByDescending(n => n.CreatedAt);
            }
        }

        public class NotificationsByTypeSpecification : BaseSpecification<EmailNotification, int>
        {
            public NotificationsByTypeSpecification(EmailNotificationType type) : base(n => n.Type == type)
            {
                AddInclude(n => n.Donation);
                AddInclude(n => n.User);
                AddOrderByDescending(n => n.CreatedAt);
            }
        }

        public class PendingNotificationsSpecification : BaseSpecification<EmailNotification, int>
        {
            public PendingNotificationsSpecification() : base(n => n.IsSent == false)
            {
                AddInclude(n => n.Donation);
                AddInclude(n => n.User);
                AddOrderBy(n => n.CreatedAt);
            }
        }

        public class SentNotificationsSpecification : BaseSpecification<EmailNotification, int>
        {
            public SentNotificationsSpecification() : base(n => n.IsSent == true)
            {
                AddInclude(n => n.Donation);
                AddInclude(n => n.User);
                AddOrderByDescending(n => n.SentAt);
            }
        }
    }
} 