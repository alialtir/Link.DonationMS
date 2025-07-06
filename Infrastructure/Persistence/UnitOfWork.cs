using Domain.Contracts;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<User> _userManager;

        public UnitOfWork(DonationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
            Campaigns = new GenericRepository<Campaign, int>(_context);
            Donations = new GenericRepository<Donation, int>(_context);
            EmailNotifications = new GenericRepository<EmailNotification, int>(_context);
            Categories = new GenericRepository<Category, int>(_context);
            Receipts = new GenericRepository<Receipt, int>(_context);
            Users = new UserRepository(_context, _userManager);
        }

        public IGenericRepository<Campaign, int> Campaigns { get; private set; }
        public IGenericRepository<Donation, int> Donations { get; private set; }
        public IGenericRepository<EmailNotification, int> EmailNotifications { get; private set; }
        public IGenericRepository<Category, int> Categories { get; private set; }
        public IGenericRepository<Receipt, int> Receipts { get; private set; }
        public IUserRepository Users { get; private set; }

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
