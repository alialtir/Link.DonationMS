using Application.Services.Abstractions;
using Domain.Contracts;
using Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Services.Specifications;
using DTOs.DonationDTOs;
using Domain.Models;
using Stripe;
using Stripe.Checkout;
using System;

namespace Application.Services
{
    /// <summary>
    /// Service for handling Stripe Webhook events
    /// </summary>
    public class StripeWebhookService : IStripeWebhookService
    {
        private readonly IConfiguration _configuration;
        private readonly ICampaignService _campaignService;
        private readonly IReceiptService _receiptService;
        private readonly INotificationService _notificationService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<StripeWebhookService> _logger;
        private readonly string _webhookSecret;

        public StripeWebhookService(
            IConfiguration configuration,
            IUnitOfWork unitOfWork,
            ICampaignService campaignService,
            IReceiptService receiptService,
            INotificationService notificationService,
            ILogger<StripeWebhookService> logger)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _campaignService = campaignService;
            _receiptService = receiptService;
            _notificationService = notificationService;
            _webhookSecret = _configuration["StripeSettings:WebhookSecret"] 
                ?? throw new InvalidOperationException("Stripe webhook secret is not configured");
        }

        /// <summary>
        /// Verifies webhook signature
        /// </summary>
        public Event VerifyWebhookSignature(string json, string signature)
        {
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json, signature, _webhookSecret);
                _logger.LogInformation("Stripe webhook signature verified successfully for event: {EventType} with ID: {EventId}", 
                    stripeEvent.Type, stripeEvent.Id);
                return stripeEvent;
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Failed to verify Stripe webhook signature");
                throw;
            }
        }

        /// <summary>
        /// Handles checkout session completed event
        /// </summary>
        public async Task<bool> HandleCheckoutSessionCompletedAsync(Event stripeEvent)
        {
            try
            {
                var session = stripeEvent.Data.Object as Session;
                if (session == null)
                {
                    _logger.LogWarning("Stripe Event does not contain a valid session object.");
                    return false;
                }

                _logger.LogInformation("Processing checkout completed for session: {SessionId}", session.Id);

                var spec = new DonationSpecifications.DonationByStripeIdsSpecification(session.Id, session.PaymentIntentId);
                var donation = await _unitOfWork.Donations.GetEntityWithSpecAsync(spec);

                if (donation == null)
                {
                    _logger.LogWarning("No donation found for session ID: {SessionId} / PaymentIntentId: {PaymentIntentId}", session.Id, session.PaymentIntentId);
                    return false;
                }

                if (donation.Status != DonationStatus.Pending)
                {
                    _logger.LogInformation("Donation {DonationId} already processed with status: {Status}", donation.Id, donation.Status);
                    return true;
                }

                await ProcessSuccessfulDonationAsync(donation, session.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing checkout session completed.");
                throw;
            }
        }

        private async Task ProcessSuccessfulDonationAsync(Donation donation, string sessionId)
        {
            donation.Status = DonationStatus.Successful;

            var progressDto = new DTOs.DonationDTOs.DonationProgressDto
            {
                CampaignId = donation.CampaignId,
                Amount = donation.Amount
            };
            await _campaignService.UpdateCampaignProgressAsync(progressDto);

            _unitOfWork.Donations.Update(donation);

            await _receiptService.GenerateReceiptAsync(donation.Id);

            // Create donation receipt notification
            try
            {
                // Get donor email from User if available

                
               
                 string donorEmail = donation.User.Email ;
                 string donorName = donation.User.DisplayNameAr;
              
                
await _notificationService.CreateNotificationAsync(
                    NotificationTypeId.DonationReceipt,
                    donorEmail,
                    new
                    {
                        
                        DonorName = donorName,
                        Amount = $"${donation.Amount:F2}",
                        CampaignName = donation.Campaign?.TitleEn ?? donation.Campaign?.TitleAr ?? "Campaign",
                        UserId = donation.UserId.GetValueOrDefault()
                    },
                    NotificationLanguage.Arabic 
                );
                
                _logger.LogInformation("Donation receipt notification created for donation {DonationId}", donation.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create donation receipt notification for donation {DonationId}", donation.Id);
                // Don't throw - notification failure shouldn't break the donation process
            }

            await _unitOfWork.CompleteAsync();

            _logger.LogInformation(
                "Donation {DonationId} marked as Successful for session {SessionId}",
                donation.Id,
                sessionId
            );
        }

        



        /// <summary>
        /// Handles payment intent payment failed event
        /// </summary>
        public async Task<bool> HandlePaymentIntentPaymentFailedAsync(Event stripeEvent)
        {
            try
            {
                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                if (paymentIntent == null)
                {
                    _logger.LogWarning("Payment intent payment failed event contains null payment intent data");
                    return false;
                }

                _logger.LogInformation("Processing payment intent payment failed event: {PaymentIntentId}", paymentIntent.Id);

                // Find donation using payment intent ID
                var spec = new DonationSpecifications.DonationByStripeIdsSpecification(
                    null, 
                    paymentIntent.Id
                );
                
                var donation = await _unitOfWork.Donations.GetEntityWithSpecAsync(spec);
                if (donation == null)
                {
                    _logger.LogWarning("No donation found for payment intent ID: {PaymentIntentId}", paymentIntent.Id);
                    return false;
                }

                // Update donation status to Failed
                if (donation.Status == DonationStatus.Pending)
                {
                    donation.Status = DonationStatus.Failed;
                    _unitOfWork.Donations.Update(donation);
                    await _unitOfWork.CompleteAsync();

                    _logger.LogInformation("Donation {DonationId} status updated to Failed for payment intent {PaymentIntentId}", 
                        donation.Id, paymentIntent.Id);
                    return true;
                }
                else
                {
                    _logger.LogInformation("Donation {DonationId} already marked as Failed", donation.Id);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling payment intent payment failed event");
                throw;
            }
        }
    }
}
