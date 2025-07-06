namespace DTOs.DashboardDTOs
{
    public class CampaignProgressDto
    {
        public int Id { get; set; }
        public string TitleAr { get; set; }
        public string TitleEn { get; set; }
        public decimal GoalAmount { get; set; }
        public decimal CurrentAmount { get; set; }
        public decimal ProgressPercentage { get; set; }
        public string Status { get; set; }
    }
} 