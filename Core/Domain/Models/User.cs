using Microsoft.AspNetCore.Identity;

namespace Domain.Models
{
    public class User : IdentityUser<Guid>
    {
        public string DisplayName { get; set; }

        public List<Donation> Donations { get; set; } 

        //public List<Campaign> Campaigns { get; set; }

    }
}