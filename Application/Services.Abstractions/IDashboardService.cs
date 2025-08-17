using DTOs.DashboardDTOs;

namespace Application.Services.Abstractions
{
    public interface IDashboardService
    {
        Task<decimal> GetTotalDonationsAsync();
        Task<int> GetTotalDonorsAsync();
        Task<IEnumerable<CampaignProgressDto>> GetTopCampaignsAsync(int? count = null);
    }
} 