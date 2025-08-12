using Application.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using DTOs.DonationDTOs;

namespace Link.DonationMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DonationsController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        public DonationsController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        // For admin only - display all donations
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1)
        {
            var result = await _serviceManager.DonationService.GetAllAsync(page);
            return Ok(result);
        }

      


       
        [HttpGet("recent-donors")] 
        [AllowAnonymous]
        public async Task<IActionResult> GetRecentDonors([FromQuery] int count = 5, [FromQuery] int? campaignId = null)
        {
            if (count <= 0) count = 5;
            var donors = await _serviceManager.DonationService.GetRecentDonorsAsync(count, campaignId);
            return Ok(donors);
        }

        [HttpGet("campaign/{campaignId}")]
        public async Task<IActionResult> GetDonationsByCampaign(int campaignId)
        {
            var result = await _serviceManager.DonationService.GetDonationsByCampaignAsync(campaignId);
            return Ok(result);
        }

        // Create donation AND generate payment checkout in one step
        [HttpPost]
        [Authorize(Roles = "Donor")]
        public async Task<IActionResult> Create([FromBody] CreateDonationDto createDonationDto)
        {
            try
            {
                var result = await _serviceManager.DonationService.CreateAsync(createDonationDto);
                // result contains paymentUrl, paymentId, status, donationId
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // Update donation status
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateDonationStatusDto updateStatusDto)
        {
            try
            {
                var result = await _serviceManager.DonationService.UpdateDonationStatusAsync(id, updateStatusDto);
                if (!result)
                {
                    return NotFound("Donation not found");
                }
                return Ok(new { message = "Status updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}