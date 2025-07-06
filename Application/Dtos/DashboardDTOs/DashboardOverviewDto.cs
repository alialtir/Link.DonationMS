namespace DTOs.DashboardDTOs
{
    public class DashboardOverviewDto
    {
        public decimal TotalDonations { get; set; }
        public int TotalDonors { get; set; }
        public int TotalCampaigns { get; set; }
        public int ActiveCampaigns { get; set; }
        public int CompletedCampaigns { get; set; }
    }
} 