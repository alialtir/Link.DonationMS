using DTOs.DashboardDTOs;
using DTOs.DonationDTOs;


namespace Application.Services.Abstractions
{
    public interface IDonationService
    {
        Task<DonationResultDto> GetByIdAsync(Guid id);
        Task<IEnumerable<DonationResultDto>> GetAllAsync(int pageNumber = 1);
        Task<IEnumerable<DonationResultDto>> GetDonationsByUserAsync(Guid userId);
        Task<IEnumerable<DonationResultDto>> GetDonationsByCampaignAsync(int campaignId);
        Task<IEnumerable<RecentDonorDto>> GetRecentDonorsAsync(int count = 5, int? campaignId = null);
        Task<DonationPaymentResultDto> CreateAsync(CreateDonationDto createDonationDto);
        Task<bool> UpdateDonationStatusAsync(Guid id, UpdateDonationStatusDto updateStatusDto);
        Task<decimal> GetTotalDonationsByCampaignAsync(int campaignId);
        Task<bool> ProcessPaymentAsync(Guid donationId, string paymentIntentId);
     
    }
} 