using Application.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Link.DonationMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class DonationsController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        public DonationsController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1)
        {
            var result = await _serviceManager.DonationService.GetAllAsync(page);
            return Ok(result);
        }
    }
} 