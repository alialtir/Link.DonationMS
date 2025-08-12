using Domain.Contracts;
using Domain.Models;
using Services.Specifications;

namespace Services.Specifications
{
    public class NotificationSpecifications
    {
        public class NotificationsByUserSpecification : BaseSpecification<Notification, int>
        {
            public NotificationsByUserSpecification(Guid userId) : base(n => n.UserId == userId)
            {
                AddInclude(n => n.User);
                AddInclude(n => n.NotificationType);
            }
        }

        public class NotificationsByTypeSpecification : BaseSpecification<Notification, int>
        {
            public NotificationsByTypeSpecification(int typeId) : base(n => n.NotificationTypeId == typeId)
            {
                AddInclude(n => n.User);
                AddInclude(n => n.NotificationType);
            }
        }

        public class PendingNotificationsSpecification : BaseSpecification<Notification, int>
        {
            public PendingNotificationsSpecification() : base(n => n.IsSent == false)
            {
                AddInclude(n => n.User);
                AddInclude(n => n.NotificationType);
            }
        }

        public class SentNotificationsSpecification : BaseSpecification<Notification, int>
        {
            public SentNotificationsSpecification() : base(n => n.IsSent == true)
            {
                AddInclude(n => n.User);
                AddInclude(n => n.NotificationType);
            }
        }

        public class NotificationsByLanguageSpecification : BaseSpecification<Notification, int>
        {
            public NotificationsByLanguageSpecification(NotificationLanguage language) : base(n => n.LanguageId == language)
            {
                AddInclude(n => n.NotificationType);
            }
        }
    }
}
