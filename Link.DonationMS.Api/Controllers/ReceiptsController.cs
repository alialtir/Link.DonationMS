using Application.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Link.DonationMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReceiptsController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        public ReceiptsController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        // GET api/receipts/my
        [HttpGet("my")]
        [Authorize(Roles = "Donor")]
        public async Task<IActionResult> GetMyReceipts()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("User ID not found");
            }

            var receipts = await _serviceManager.ReceiptService.GetByUserAsync(userId);
            return Ok(receipts);
        }

        // GET api/receipts/{id}/pdf
        [HttpGet("{id}/pdf")]
        [Authorize(Roles = "Donor")]
        public async Task<IActionResult> DownloadPdf(int id)
        {
            var pdfBytes = await _serviceManager.ReceiptService.GeneratePdfAsync(id);
            if (pdfBytes == null)
                return NotFound();

            return File(pdfBytes, "application/pdf", $"receipt_{id}.pdf");
        }
    }
}
