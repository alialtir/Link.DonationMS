using Application.Services.Abstractions;
using DTOs.EmailNotificationDTOs;

namespace Services
{
    public class EmailNotificationService : IEmailNotificationService
    {
 

        public Task<bool> SendCampaignGoalReachedEmailAsync(int campaignId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SendDonationReceiptEmailAsync(int donationId)
        {
            throw new NotImplementedException();
        }
    }
} 