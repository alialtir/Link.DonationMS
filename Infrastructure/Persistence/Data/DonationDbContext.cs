using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenIddict.EntityFrameworkCore.Models;
using OpenIddict.EntityFrameworkCore;

namespace Persistence.Data
{
    public class DonationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public DonationDbContext(DbContextOptions<DonationDbContext> options) : base(options)
        {
        }

        public DbSet<Campaign> Campaigns { get; set; }

        public DbSet<Donation> Donations { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Receipt> Receipts { get; set; }

        public DbSet<EmailNotification> EmailNotifications { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AssemblyRef).Assembly);
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.UseOpenIddict<Guid>();
        }

     
    
    }
}
