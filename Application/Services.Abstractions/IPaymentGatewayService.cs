using DTOs.PaymentDTOs;

namespace Application.Services.Abstractions
{
    public interface IPaymentGatewayService
    {
        /// <summary>
        /// Generates a payment object with payment URL, payment ID, and status
        /// </summary>
        /// <param name="donationId">The donation ID</param>
        /// <param name="amount">The donation amount</param>
        /// <param name="currency">The currency (default: USD)</param>
        /// <returns>Payment object containing URL, ID, and status</returns>
        Task<PaymentObjectDto> GeneratePaymentObjectAsync(Guid donationId, decimal amount, string? currency = null);
    }
}
