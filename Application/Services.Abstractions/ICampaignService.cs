using DTOs.CampaignDTOs;

namespace Application.Services.Abstractions
{
    public interface ICampaignService
    {
        Task<CampaignResultDto> GetByIdAsync(int id);
        Task<IEnumerable<CampaignResultDto>> GetAllAsync();
        Task<IEnumerable<CampaignResultDto>> GetActiveCampaignsAsync();
        Task<IEnumerable<CampaignResultDto>> GetCompletedCampaignsAsync();
        Task<IEnumerable<CampaignResultDto>> GetCampaignsByCategoryAsync(int categoryId);
        Task<IEnumerable<CampaignResultDto>> SearchCampaignsByTitleAsync(string title);
        Task<CampaignResultDto> CreateAsync(CreateCampaignDto createCampaignDto);
        Task<CampaignResultDto> UpdateAsync(int id, UpdateCampaignDto updateCampaignDto);
        Task<bool> DeleteAsync(int id);
        Task<decimal> GetCampaignProgressAsync(int id);
    }
} 