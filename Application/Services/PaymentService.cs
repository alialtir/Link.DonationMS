//using Application.Services.Abstractions;
//using DTOs.DonationDTOs;
//using DTOs.PaymentDTOs;
//using Microsoft.Extensions.Configuration;
//using Stripe;
//using Stripe.Checkout;
//using Domain.Contracts;
//using AutoMapper;
//using Domain.Models;
//using System.Collections.Generic;
//using System;
//using System.Threading.Tasks;
//using System.Linq;


//namespace Services
//{
//    public class PaymentService : IPaymentService
//    {
//        private readonly IConfiguration _configuration;
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IMapper _mapper;
//        private readonly SessionService _sessionService;

//        private readonly IPaymentGatewayService _gatewayService;

//        public PaymentService(
//            IConfiguration configuration,
//            IUnitOfWork unitOfWork,
//            IMapper mapper,
//            IPaymentGatewayService gatewayService)
//        {
//            _configuration = configuration;
//            _unitOfWork = unitOfWork;
//            _mapper = mapper;
//            _gatewayService = gatewayService;
//            StripeConfiguration.ApiKey = _configuration["StripeSettings:SecretKey"];
//            _sessionService = new SessionService();
//        }

//        /// <summary>
//        /// Creates a Stripe Checkout session for a specific donation.
//        /// </summary>
//        /// <param name="donationId">The donation ID.</param>
//        /// <returns>Session URL for user redirection.</returns>
//        public async Task<PaymentObjectDto> GeneratePaymentObjectAsync(int donationId, decimal amount, string currency = "USD")
//        {
            
//            try
//            {
//                // Retrieve the donation from the database
//                var donation = await _unitOfWork.Donations.GetByIdAsync(donationId);
//                if (donation == null || donation.Status != DonationStatus.Pending)
//                {
//                    var errorMessage = $"Donation with id {donationId} is not valid for checkout.";
//                    throw new ArgumentException(errorMessage);
//                }

//            // Configure session options
//            var options = new SessionCreateOptions
//            {
//                PaymentMethodTypes = new List<string> { "card" },
//                Mode = "payment",
//                Metadata = new Dictionary<string, string>
//                {
//                    { "donationId", donationId.ToString() }
//                },
//                LineItems = new List<SessionLineItemOptions>
//                {
//                    new SessionLineItemOptions
//                    {
//                        PriceData = new SessionLineItemPriceDataOptions
//                        {
//                            Currency = _configuration["StripeSettings:Currency"] ?? "usd",
//                            UnitAmount = (long)(donation.Amount * 100), // Stripe works with the smallest currency unit
//                            ProductData = new SessionLineItemPriceDataProductDataOptions
//                            {
//                                Name = $"Donation #{donationId}"
//                            }
//                        },
//                        Quantity = 1
//                    }
//                },
//                SuccessUrl = _configuration["StripeSettings:SuccessUrl"],
//                CancelUrl = _configuration["StripeSettings:CancelUrl"]
//            };

//                var session = await _sessionService.CreateAsync(options);

//                // Save session and payment intent IDs
//                donation.PaymentIntentId = session.PaymentIntentId;
//                donation.StripeSessionId = session.Id;
//                _unitOfWork.Donations.Update(donation);
//                await _unitOfWork.CompleteAsync();

//                var paymentObj = new PaymentObjectDto
//                {
//                    PaymentUrl = session.Url,
//                    PaymentId = session.PaymentIntentId,
//                    Status = PaymentObjectStatus.Success
//                };
//                return paymentObj;
//            }
//            catch (Exception ex)
//            {

//                return new PaymentObjectDto
//                {
//                    Status = PaymentObjectStatus.Failed,
//                    ErrorMessage = ex.Message
//                };
//            }
//        }

//        public async Task<string> CreateCheckoutSessionAsync(int donationId)
//        {
//            var result = await GeneratePaymentObjectAsync(donationId, 0);
//            return result.PaymentUrl;
//        }

//        public async Task<bool> ConfirmPaymentAsync(string sessionId)
//        {
//            if (string.IsNullOrWhiteSpace(sessionId)) return false;

//            // Get the session from Stripe
//            var session = await _sessionService.GetAsync(sessionId);
//            if (session == null) return false;

//            // Get the matching donation via SessionId or PaymentIntentId
//            var spec = new Specifications.DonationSpecifications.DonationByStripeIdsSpecification(session.Id, session.PaymentIntentId);
//            var donation = await _unitOfWork.Donations.GetEntityWithSpecAsync(spec);
//            if (donation == null) return false;

//            // Get PaymentIntent to verify status
//            var paymentIntentService = new PaymentIntentService();
//            var paymentIntent = await paymentIntentService.GetAsync(session.PaymentIntentId);
//            if (paymentIntent == null) return false;

//            if (paymentIntent.Status == "succeeded")
//            {
//                donation.Status = DonationStatus.Successful;
//            }
//            else if (paymentIntent.Status == "processing" || paymentIntent.Status == "requires_payment_method")
//            {
//                donation.Status = DonationStatus.Pending;
//            }
//            else
//            {
//                donation.Status = DonationStatus.Failed;
//            }

//            _unitOfWork.Donations.Update(donation);
//            await _unitOfWork.CompleteAsync();
//            return true;
//        }

     
//    }
//}