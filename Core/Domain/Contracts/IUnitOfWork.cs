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
        IGenericRepository<Donation, int> Donations { get; }
        IGenericRepository<EmailNotification, int> EmailNotifications { get; }
        IGenericRepository<Category, int> Categories { get; }
        

        Task<int> CompleteAsync();
    }


}
