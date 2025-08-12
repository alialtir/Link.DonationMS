using DTOs.DashboardDTOs;
using DTOs.CampaignDTOs;

namespace Link.DonationMS.AdminPortal.Models.ViewModels
{
    public class DashboardViewModel
    {
        public DashboardStatsDto Overview { get; set; }
        public List<CampaignProgressDto> TopCampaigns { get; set; }
    }
}
