using DTOs.EmailNotificationDTOs;

namespace Application.Services.Abstractions
{
    public interface IEmailNotificationService
    {
        Task<EmailNotificationDto> GetByIdAsync(int id);
        Task<IEnumerable<EmailNotificationDto>> GetAllAsync();
        Task<EmailNotificationDto> CreateAsync(CreateEmailNotificationDto createNotificationDto);
        Task<bool> SendDonationReceiptEmailAsync(int donationId);
        Task<bool> SendCampaignGoalReachedEmailAsync(int campaignId);
    }
} 