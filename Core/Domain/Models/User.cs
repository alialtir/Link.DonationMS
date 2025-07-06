using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Domain.Models
{
    public class User : IdentityUser<Guid>
    {
        public string DisplayName { get; set; }

        public List<Donation> Donations { get; set; } = new List<Donation>();
    }
}