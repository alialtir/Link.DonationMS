using Application.Services.Abstractions;
using DTOs.DashboardDTOs;

namespace Services
{
    public class DashboardService : IDashboardService
    {
        public Task<IEnumerable<DonorHistoryDto>> GetDonorHistoryAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CampaignProgressDto>> GetTopCampaignsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<decimal> GetTotalDonationsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<int> GetTotalDonorsAsync()
        {
            throw new NotImplementedException();
        }
    }
} 