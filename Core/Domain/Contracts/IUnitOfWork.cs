using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Campaign, int> Campaigns { get; }
        IGenericRepository<Donation, Guid> Donations { get; }
        IGenericRepository<Category, int> Categories { get; }
        IGenericRepository<Notification, int> Notifications { get; }
        IGenericRepository<NotificationType, int> NotificationTypes { get; }
        IGenericRepository<Receipt, int> Receipts { get; }
        IUserRepository Users { get; }

        Task<int> CompleteAsync();
    }
}
