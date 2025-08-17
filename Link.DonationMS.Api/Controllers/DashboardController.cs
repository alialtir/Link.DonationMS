using DTOs.DashboardDTOs;
using Application.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Link.DonationMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public DashboardController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

  
        [HttpGet("overview")]
        public async Task<ActionResult<DashboardStatsDto>> GetOverviewAsync()
        {
            var totalDonations = await _serviceManager.DashboardService.GetTotalDonationsAsync();
            var totalDonors = await _serviceManager.DashboardService.GetTotalDonorsAsync();

            var result = new DashboardStatsDto
            {
                TotalDonations = totalDonations,
                TotalDonors = totalDonors
            };

            return Ok(result);
        }

     
        [HttpGet("top-campaigns")]
        public async Task<ActionResult<IEnumerable<CampaignProgressDto>>> GetTopCampaignsAsync([FromQuery] int? count = null)
        {
            var campaigns = await _serviceManager.DashboardService.GetTopCampaignsAsync(count);
            return Ok(campaigns);
        }
    }
}
