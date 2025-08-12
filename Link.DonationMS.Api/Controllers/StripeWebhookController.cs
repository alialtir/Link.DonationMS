using Microsoft.AspNetCore.Mvc;
using Stripe;
using Application.Services.Abstractions;

namespace Link.DonationMS.Api.Controllers
{
    /// <summary>
    /// Controller for handling Stripe Webhook events
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class StripeWebhookController : ControllerBase
    {
        private readonly IStripeWebhookService _stripeWebhookService;
        private readonly ILogger<StripeWebhookController> _logger;

        public StripeWebhookController(
            IStripeWebhookService stripeWebhookService,
            ILogger<StripeWebhookController> logger)
        {
            _stripeWebhookService = stripeWebhookService;
            _logger = logger;
        }

        /// <summary>
        /// Handles Stripe Webhook events
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> HandleWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            
            try
            {
                // Verify webhook signature
                var stripeEvent = _stripeWebhookService.VerifyWebhookSignature(
                    json,
                    Request.Headers["Stripe-Signature"]
                );

                _logger.LogInformation("Received Stripe webhook event: {EventType} with ID: {EventId}", 
                    stripeEvent.Type, stripeEvent.Id);

                // Handle event based on type
                bool handled = stripeEvent.Type switch
                {
                    "checkout.session.completed" => await _stripeWebhookService.HandleCheckoutSessionCompletedAsync(stripeEvent),
                    "payment_intent.payment_failed" => await _stripeWebhookService.HandlePaymentIntentPaymentFailedAsync(stripeEvent),
                    _ => LogUnhandledEvent(stripeEvent.Type)
                };

                return Ok(new { success = true, handled });
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Failed to verify Stripe webhook signature");
                return BadRequest("Invalid signature");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Stripe webhook");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Logs unhandled events
        /// </summary>
        private bool LogUnhandledEvent(string eventType)
        {
            _logger.LogInformation("Unhandled Stripe event type: {EventType}", eventType);
            return false;
        }
    }
}
