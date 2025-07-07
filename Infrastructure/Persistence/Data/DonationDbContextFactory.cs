using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Persistence.Data
{
    public class DonationDbContextFactory : IDesignTimeDbContextFactory<DonationDbContext>
    {
        public DonationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DonationDbContext>();
            
       
            var connectionString = "Server=DELL\\SQLEXPRESS;Database=DonationMSDb;Trusted_Connection=True;TrustServerCertificate=True";
            
            optionsBuilder.UseSqlServer(connectionString);

            return new DonationDbContext(optionsBuilder.Options);
        }
    }
} 