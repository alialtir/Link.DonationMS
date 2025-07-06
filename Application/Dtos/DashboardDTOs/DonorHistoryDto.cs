using System;

namespace DTOs.DashboardDTOs
{
    public class DonorHistoryDto
    {
        public int DonationId { get; set; }
        public decimal Amount { get; set; }
        public DateTime DonationDate { get; set; }
        public string CampaignTitle { get; set; }
        public string Status { get; set; }
    }
} 