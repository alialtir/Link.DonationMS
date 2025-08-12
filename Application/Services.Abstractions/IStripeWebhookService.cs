using Stripe;

namespace Application.Services.Abstractions
{
    /// <summary>
    /// Service for handling Stripe Webhook events
    /// </summary>
    public interface IStripeWebhookService
    {
        /// <summary>
        /// Handles checkout session completed event
        /// </summary>
        /// <param name="stripeEvent">Stripe event</param>
        /// <returns>Processing result</returns>
        Task<bool> HandleCheckoutSessionCompletedAsync(Event stripeEvent);

        /// <summary>
        /// Handles payment intent payment failed event
        /// </summary>
        /// <param name="stripeEvent">Stripe event</param>
        /// <returns>Processing result</returns>
        Task<bool> HandlePaymentIntentPaymentFailedAsync(Event stripeEvent);

        /// <summary>
        /// Verifies webhook signature
        /// </summary>
        /// <param name="json">JSON content</param>
        /// <param name="signature">Signature</param>
        /// <returns>Verified Stripe event</returns>
        Event VerifyWebhookSignature(string json, string signature);
    }
}
