using DTOs.EmailNotificationDTOs;

namespace Application.Services.Abstractions
{
    public interface IEmailNotificationService
    {
        Task<bool> SendDonationReceiptEmailAsync(int donationId);
        Task<bool> SendCampaignGoalReachedEmailAsync(int campaignId);
    }
} 