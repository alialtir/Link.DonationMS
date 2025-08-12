using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.ReceiptDTOs
{
    public class ReceiptDto
    {
        public int Id { get; set; }
        public Guid DonationId { get; set; }
        public int? CampaignId { get; set; }
        public DateTime CreatedAt { get; set; }

        public decimal Amount { get; set; }

        public string CampaignTitleAr { get; set; }

        public string CampaignTitleEn { get; set; }

        public string ReceiptNumber { get; set; }
    }
}
