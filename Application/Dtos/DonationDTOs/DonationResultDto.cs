using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.DonationDTOs
{
    public class DonationResultDto
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public bool IsAnonymous { get; set; }
        public DateTime DonationDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public int CampaignId { get; set; }
        public string CampaignTitle { get; set; } = string.Empty;
        public Guid? UserId { get; set; }
        public string? DonorName { get; set; }
    }
}
