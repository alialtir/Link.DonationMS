using DTOs.DonationDTOs;
using DTOs.PaymentDTOs;

namespace Application.Services.Abstractions
{
    public interface IPaymentService
    {
        /// <summary>
        /// Generates a payment object (URL, ID, status) via underlying gateway.
        /// </summary>
        /// <param name="donationId">The donation ID.</param>
        /// <param name="amount">Donation amount.</param>
        /// <param name="currency">Currency code.</param>
        /// <returns>Payment object DTO.</returns>
        Task<PaymentObjectDto> GeneratePaymentObjectAsync(Guid donationId, decimal amount, string currency = "USD");

        /// <summary>
        /// Creates a Stripe Checkout session and returns the redirection URL.
        /// (Deprecated: use GeneratePaymentObjectAsync instead)
        /// </summary>
        Task<string> CreateCheckoutSessionAsync(Guid donationId);

        /// <summary>
        /// Verifies the payment status with Stripe and updates the donation.
        /// </summary>
        /// <param name="sessionId">The Checkout session ID.</param>
        /// <returns>True if the update was successful.</returns>
        Task<bool> ConfirmPaymentAsync(string sessionId);
    }
}