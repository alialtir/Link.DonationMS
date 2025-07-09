using Application.Services.Abstractions;
using DTOs.DonationDTOs;
using DTOs.PaymentDTOs;

namespace Services
{
    public class PaymentService : IPaymentService
    {
        public Task<PaymentIntentDto> CreateOrUpdatePaymentIntentAsync(int donationId)
        {
            throw new NotImplementedException();
        }

        public Task<PaymentIntentDto> CreateOrUpdatePaymentIntentAsync(CreateDonationDto createDonationDto)
        {
            throw new NotImplementedException();
        }
    }
} 