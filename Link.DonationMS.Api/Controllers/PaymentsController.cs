//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Application.Services.Abstractions;
//using DTOs.PaymentDTOs;

//namespace Link.DonationMS.Api.Controllers
//{
//    // PaymentsController has been deprecated since unified donation/payment endpoint.

//    [Route("api/[controller]")]
//    [ApiController]
//    //[Authorize] // Ensure user is logged in
//    public class PaymentsController : ControllerBase
//    {
//        private readonly IPaymentService _paymentService;
//        private readonly ILogger<PaymentsController> _logger;

//        public PaymentsController(IPaymentService paymentService, ILogger<PaymentsController> logger)
//        {
//            _paymentService = paymentService;
//            _logger = logger;
//        }

//        [HttpPost("checkout/{donationId}")]
//        public async Task<ActionResult<string>> CreateCheckout(int donationId)
//        {
//            try
//            {
//                _logger.LogInformation("Creating new payment session for donation: {DonationId}", donationId);
//                var checkoutUrl = await _paymentService.CreateCheckoutSessionAsync(donationId);
//                return Ok(new { CheckoutUrl = checkoutUrl });
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error creating checkout session for donation {DonationId}", donationId);
//                return BadRequest(new { 
//                    error = "Bad Request",
//                    message = "An error occurred while creating the payment session"
//                });
//            }
//        }

//        [AllowAnonymous] // Allow anonymous access because Stripe will send the request
//        [HttpGet("confirm")]
//        public async Task<IActionResult> ConfirmPayment([FromQuery] string sessionId)
//        {
//            try
//            {
//                var result = await _paymentService.ConfirmPaymentAsync(sessionId);
//                var redirectUrl = result 
//                    ? "/?payment=success" 
//                    : "/?payment=failed";
                
//                return Redirect(redirectUrl);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error confirming payment for session {SessionId}", sessionId);
//                return Redirect("/?payment=error");
//            }
//        }
//    }
//}
