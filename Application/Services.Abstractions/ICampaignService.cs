using DTOs.CampaignDTOs;
using DTOs.DonationDTOs;

namespace Application.Services.Abstractions
{
    public interface ICampaignService
    {


        Task<CampaignResultDto> GetByIdAsync(int id);
        Task<IEnumerable<CampaignResultDto>> GetAllAsync(int pageNumber = 1, int pageSize = 6);
        Task<int> GetCountAsync();
        Task<IEnumerable<CampaignResultDto>> GetCompletedCampaignsAsync();
        Task<IEnumerable<CampaignResultDto>> GetCampaignsByCategoryAsync(int categoryId);
        Task<IEnumerable<CampaignResultDto>> SearchCampaignsByTitleAsync(string title);
        Task<CampaignResultDto> CreateAsync(CreateCampaignDto createCampaignDto);
        Task<CampaignResultDto> UpdateAsync(int id, UpdateCampaignDto updateCampaignDto);
        Task<bool> DeleteAsync(int id);
        Task<decimal> GetCampaignProgressAsync(int id);
        Task<IEnumerable<CampaignResultDto>> GetActiveCampaignsFilteredAsync(string title = null, int? categoryId = null, int pageNumber = 1, int pageSize = 5);
        Task UpdateCampaignProgressAsync(DTOs.DonationDTOs.DonationProgressDto donationProgress);
    }
}