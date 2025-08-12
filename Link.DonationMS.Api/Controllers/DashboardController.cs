using DTOs.DashboardDTOs;
using Application.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Link.DonationMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

  
        [HttpGet("overview")]
        public async Task<ActionResult<DashboardStatsDto>> GetOverviewAsync()
        {
            var totalDonations = await _dashboardService.GetTotalDonationsAsync();
            var totalDonors = await _dashboardService.GetTotalDonorsAsync();

            var result = new DashboardStatsDto
            {
                TotalDonations = totalDonations,
                TotalDonors = totalDonors
            };

            return Ok(result);
        }

     
        [HttpGet("top-campaigns")]
        public async Task<ActionResult<IEnumerable<CampaignProgressDto>>> GetTopCampaignsAsync()
        {
            var campaigns = await _dashboardService.GetTopCampaignsAsync();
            return Ok(campaigns);
        }
    }
}
