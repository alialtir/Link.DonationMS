using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.DonationDTOs
{
    public class CreateDonationDto
    {
        [Required]
        public decimal Amount { get; set; }
        public bool IsAnonymous { get; set; } = false;

        [Required]
        public int CampaignId { get; set; }

    
    }
}
