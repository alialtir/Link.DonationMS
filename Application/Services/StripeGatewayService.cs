using Application.Services.Abstractions;
using DTOs.PaymentDTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Stripe;
using Stripe.Checkout;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public class StripeGatewayService : IPaymentGatewayService
    {
        private readonly IConfiguration _configuration;
        private readonly SessionService _sessionService;
        private readonly ILogger<StripeGatewayService> _logger;

        public StripeGatewayService(IConfiguration configuration, ILogger<StripeGatewayService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            StripeConfiguration.ApiKey = _configuration["StripeSettings:SecretKey"];
            _sessionService = new SessionService();
        }

        public async Task<PaymentObjectDto> GeneratePaymentObjectAsync(Guid donationId, decimal amount, string? currency = null)
        {
            try
            {
                // Resolve configurable settings
                var usedCurrency = string.IsNullOrWhiteSpace(currency)
                    ? (_configuration["StripeSettings:Currency"] ?? "usd")
                    : currency.ToLower();

                var paymentMethodsCfg = _configuration["StripeSettings:PaymentMethodTypes"];

                var paymentMethods = !string.IsNullOrWhiteSpace(paymentMethodsCfg)
                    ? paymentMethodsCfg.Split(',').Select(m => m.Trim()).Where(m=>!string.IsNullOrWhiteSpace(m)).ToList()
                    : new List<string> { "card" };


                var sessionMode = _configuration["StripeSettings:Mode"] ?? "payment";


                var productTemplate = _configuration["StripeSettings:ProductNameTemplate"] ?? "Donation #{donationId}";

                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = paymentMethods,
                    Mode = sessionMode,
                    Metadata = new Dictionary<string, string>
                    {
                        { "donationId", donationId.ToString() }
                    },
                    LineItems = new List<SessionLineItemOptions>
                    {
                        new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                Currency = usedCurrency,
                                UnitAmount = (long)(amount * 100),
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = productTemplate.Replace("{donationId}", donationId.ToString())
                                }
                            },
                            Quantity = 1
                        }
                    },
                    SuccessUrl = _configuration["StripeSettings:SuccessUrl"],
                    CancelUrl = _configuration["StripeSettings:CancelUrl"]
                };

                var session = await _sessionService.CreateAsync(options);

                return new PaymentObjectDto
                {
                    PaymentUrl = session.Url,
                    PaymentId = session.Id,
                    Status = PaymentObjectStatus.Success
                };
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Stripe checkout session creation failed for donation {DonationId}", donationId);
                return new PaymentObjectDto
                {
                    PaymentUrl = null,
                    PaymentId = null,
                    Status = PaymentObjectStatus.Failed,
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}
