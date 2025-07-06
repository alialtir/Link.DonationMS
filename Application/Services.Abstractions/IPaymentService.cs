using DTOs.DonationDTOs;
using DTOs.PaymentDTOs;

namespace Application.Services.Abstractions
{
    public interface IPaymentService
    {
        Task<PaymentIntentDto> CreateOrUpdatePaymentIntentAsync(int donationId);
        Task<PaymentIntentDto> CreateOrUpdatePaymentIntentAsync(CreateDonationDto createDonationDto);
    }
} 