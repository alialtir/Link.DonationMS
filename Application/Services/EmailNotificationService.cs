using Application.Services.Abstractions;
using DTOs.EmailNotificationDTOs;

namespace Services
{
    public class EmailNotificationService : IEmailNotificationService
    {
        public Task<EmailNotificationDto> CreateAsync(CreateEmailNotificationDto createNotificationDto)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<EmailNotificationDto>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<EmailNotificationDto> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

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