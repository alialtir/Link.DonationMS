using DTOs.DonationDTOs;


namespace Application.Services.Abstractions
{
    public interface IDonationService
    {
        Task<DonationResultDto> GetByIdAsync(int id);
        Task<IEnumerable<DonationResultDto>> GetAllAsync(int pageNumber = 1);
        Task<IEnumerable<DonationResultDto>> GetDonationsByUserAsync(Guid userId);
        Task<IEnumerable<DonationResultDto>> GetDonationsByCampaignAsync(int campaignId);
        Task<DonationResultDto> CreateAsync(CreateDonationDto createDonationDto);
        Task<bool> UpdateDonationStatusAsync(int id, UpdateDonationStatusDto updateStatusDto);
        Task<bool> DeleteAsync(int id);
        Task<decimal> GetTotalDonationsByCampaignAsync(int campaignId);
        Task<bool> ProcessPaymentAsync(int donationId, string paymentIntentId);
        Task<bool> GenerateReceiptAsync(int donationId);
    }
} 