using Domain.Contracts;
using Domain.Models;
using Persistence.Data;
using Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DonationDbContext _context;

        public UnitOfWork(DonationDbContext context)
        {
            _context = context;
            Campaigns = new GenericRepository<Campaign, int>(_context);
            Donations = new GenericRepository<Donation, int>(_context);
            EmailNotifications = new GenericRepository<EmailNotification, int>(_context);
            Categories = new GenericRepository<Category, int>(_context);
          
        }

        public IGenericRepository<Campaign, int> Campaigns { get; private set; }
        public IGenericRepository<Donation, int> Donations { get; private set; }
        public IGenericRepository<EmailNotification, int> EmailNotifications { get; private set; }
        public IGenericRepository<Category, int> Categories { get; private set; }
      

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }

}
